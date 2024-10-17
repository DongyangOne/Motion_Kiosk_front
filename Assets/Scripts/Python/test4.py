import cv2
import time
import numpy as np
import hand_detector as hd
import pyautogui
import socket
import pickle
import struct

print("실행함.")

# 화면 해상도와 설정값 정의
wCam, hCam = 640, 480
frameR = 100  # 프레임 경계값
smoothening = 3  # 부드럽게 이동시키기 위한 값

# 시간 및 좌표 설정
pTime = 0
plocX, plocY = 0, 0
clocX, clocY = 0, 0

# 소켓 설정 (서버 역할을 수행)
server_socket = socket.socket(socket.AF_INET, socket.SOCK_STREAM)
server_socket.bind(('0.0.0.0', 8089))  # 모든 인터페이스에서 수신
server_socket.listen(5)
conn, addr = server_socket.accept()

# 웹캠 열기
cap = cv2.VideoCapture(0)
cap.set(3, wCam)  # 화면 가로 크기 설정
cap.set(4, hCam)  # 화면 세로 크기 설정

detector = hd.handDetector(detectionCon=0.7)  # 손 추적기 초기화
wScr, hScr = pyautogui.size()  # 화면 해상도 가져오기
print(wScr, hScr)

pyautogui.FAILSAFE = False  # PyAutoGUI의 예외 방지

while True:
    success, img = cap.read()  # 웹캠에서 이미지 읽기
    img = detector.findHands(img)  # 손을 감지 및 표시
    lmList, bbox = detector.findPosition(img)  # 손 랜드마크 및 경계상자 찾기
    output = img.copy()

    if len(lmList) != 0:
        # 손바닥 중앙 좌표를 기준으로 마우스 이동
        cx, cy = detector.findPalmCenter()

        if cx and cy:
            x3 = np.interp(cx, (frameR, wCam - frameR), (0, wScr))  # X좌표 변환
            y3 = np.interp(cy, (frameR, hCam - frameR), (0, hScr))  # Y좌표 변환

            # 부드럽게 좌표 이동
            clocX = plocX + (x3 - plocX) / smoothening
            clocY = plocY + (y3 - plocY) / smoothening

            # 마우스 이동
            pyautogui.moveTo(wScr - clocX, clocY)
            cv2.circle(img, (cx, cy), 6, (255, 28, 0), cv2.FILLED)
            plocX, plocY = clocX, clocY

            # 손바닥 좌표 전송 (cx, cy 값을 전송)
            hand_data = {
                'cursor': (wScr - clocX, clocY),  # 좌표 정보
                'click': 0  # 주먹을 쥐지 않은 상태
            }

        # 주먹이 쥐어진 상태일 때 클릭 발생
        if detector.isFist():
            pyautogui.click()
            hand_data['click'] = 1  # 주먹을 쥐었을 때 클릭 신호 전송

        # 손바닥 좌표와 클릭 상태 전송
        data = pickle.dumps(hand_data)
        message_size = struct.pack("L", len(data))
        conn.sendall(message_size + data)

    # FPS 계산
    cTime = time.time()
    fps = 1 / (cTime - pTime)
    pTime = cTime

    # 이미지 출력
    cv2.imshow("Vitual mouse monitor", cv2.flip(img, 1))
    cv2.setWindowProperty("Vitual mouse monitor", cv2.WND_PROP_TOPMOST, 1)
    cv2.waitKey(1)

cap.release()
conn.close()
server_socket.close()
