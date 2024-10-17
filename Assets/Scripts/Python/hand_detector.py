import cv2
import time
import numpy as np
import mediapipe as mp
import pyautogui

# 손 추적 클래스 정의
class handDetector:
    def __init__(self, mode=False, maxHands=2, modelComplexity=1, detectionCon=0.7, trackCon=0.7):
        self.mode = mode
        self.maxHands = maxHands
        self.modelComplex = modelComplexity
        self.detectionCon = detectionCon
        self.trackCon = trackCon

        # Mediapipe hands 초기화
        self.mpHands = mp.solutions.hands
        self.hands = self.mpHands.Hands(self.mode, self.maxHands, self.modelComplex, self.detectionCon, self.trackCon)
        self.mpDraw = mp.solutions.drawing_utils
        self.tipIds = [4, 8, 12, 16, 20]  # 손가락 끝 랜드마크 IDs
        self.results = None
        self.lmList = []
        self.handId = None  # 첫 번째 손의 ID를 저장

    def findHands(self, img, draw=True):
        # Mediapipe에서 사용할 수 있도록 이미지를 RGB로 변환
        imgRGB = cv2.cvtColor(img, cv2.COLOR_BGR2RGB)
        self.results = self.hands.process(imgRGB)

        if self.results.multi_hand_landmarks:
            for handNo, handLms in enumerate(self.results.multi_hand_landmarks):
                if self.handId is None:  # 처음으로 인식된 손의 ID를 고정
                    self.handId = handNo
                if handNo == self.handId:  # 고정된 손만 처리
                    if draw:
                        self.mpDraw.draw_landmarks(img, handLms, self.mpHands.HAND_CONNECTIONS)
        return img

    def findPosition(self, img, draw=True):
        self.lmList = []
        bbox = None

        if self.results.multi_hand_landmarks:
            myHand = self.results.multi_hand_landmarks[self.handId]

            for id, lm in enumerate(myHand.landmark):
                h, w, c = img.shape
                cx, cy = int(lm.x * w), int(lm.y * h)
                self.lmList.append([id, cx, cy])

                if draw:
                    cv2.circle(img, (cx, cy), 6, (0, 0, 255), cv2.FILLED)

            # bbox 좌표 계산
            xList = [lm[1] for lm in self.lmList]
            yList = [lm[2] for lm in self.lmList]
            bbox = (min(xList), min(yList), max(xList), max(yList))

        return self.lmList, bbox

    def fingersUp(self):
        """손가락이 접혀 있는지 확인하는 함수"""
        fingers = []
        if len(self.lmList) > 0:
            # Thumb: 손가락 끝과 손가락 중간 관절을 비교하여 접힘 상태 확인
            if self.lmList[self.tipIds[0]][1] < self.lmList[self.tipIds[0] - 1][1]:  # 엄지
                fingers.append(1)
            else:
                fingers.append(0)

            # 다른 손가락들: 손가락 끝과 중간 관절을 비교하여 접힘 상태 확인
            for id in range(1, 5):
                if self.lmList[self.tipIds[id]][2] < self.lmList[self.tipIds[id] - 2][2]:  # 검지부터 새끼손가락
                    fingers.append(1)
                else:
                    fingers.append(0)

        return fingers

    def isFist(self):
        """주먹이 쥐어졌는지 확인하는 함수"""
        fingers = self.fingersUp()
        if fingers == [0, 0, 0, 0, 0]:  # 모든 손가락이 접혀있으면 주먹
            return True
        return False

    def findPalmCenter(self):
        """손바닥 중앙 좌표 계산"""
        if len(self.lmList) > 0:
            # 손목(0번 랜드마크)과 중지(9번 랜드마크)의 평균을 손바닥 중앙으로 간주
            x1, y1 = self.lmList[0][1:]  # 손목
            x2, y2 = self.lmList[9][1:]  # 중지 아래쪽 랜드마크
            cx, cy = (x1 + x2) // 2, (y1 + y2) // 2
            return cx, cy
        return None, None

# 메인 코드
print("실행함.")

# 화면 해상도와 설정값 정의
wCam, hCam = 640, 480
frameR = 100  # 프레임 경계값
smoothening = 3  # 부드럽게 이동시키기 위한 값

# 시간 및 좌표 설정
pTime = 0
plocX, plocY = 0, 0
clocX, clocY = 0, 0

cap = cv2.VideoCapture(0)  # 웹캠 열기
cap.set(3, wCam)  # 화면 가로 크기 설정
cap.set(4, hCam)  # 화면 세로 크기 설정

detector = handDetector(detectionCon=0.7)  # 손 추적기 초기화
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

        # 주먹이 쥐어진 상태일 때 클릭 발생
        if detector.isFist():
            pyautogui.click()

    # FPS 계산
    cTime = time.time()
    fps = 1 / (cTime - pTime)
    pTime = cTime

    # 이미지 출력
    cv2.imshow("Vitual mouse monitor", cv2.flip(img, 1))
    cv2.setWindowProperty("Vitual mouse monitor", cv2.WND_PROP_TOPMOST, 1)
    
    # 종료 조건: 키보드 입력 시 종료
    if cv2.waitKey(1) != -1:
        break

cap.release()
cv2.destroyAllWindows()
