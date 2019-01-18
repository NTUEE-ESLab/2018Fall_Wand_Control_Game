
# import the necessary packages
from picamera.array import PiRGBArray
from picamera import PiCamera
import time
import cv2
import numpy as np
import socket
import math




s = socket.socket()
host = '192.168.43.157'#'192.168.0.171' #ip of raspberry pi
port = 12345
s.bind((host, port))

s.listen(5)

c, addr = s.accept()
print ('Got connection from',addr)
 
# initialize the camera and grab a reference to the raw camera capture
camera = PiCamera()
camera.resolution = (960, 540)
camera.framerate = 50
rawCapture = PiRGBArray(camera, size=(960, 540))


 
# allow the camera to warmup
time.sleep(0.1)
 
# capture frames from the camera
for frame in camera.capture_continuous(rawCapture, format="bgr", use_video_port=True):
	# grab the raw NumPy array representing the image, then initialize the timestamp
	# and occupied/unoccupied text



        
        

	
	image = frame.array

	lower = np.array([50, 100, 50])
        upper = np.array([100, 266, 100])
        filtered = cv2.inRange(image, lower, upper)
        blurred = cv2.GaussianBlur(filtered, (15, 15), 0)

        (_, cnts, _) = cv2.findContours(blurred.copy(), cv2.RETR_EXTERNAL, cv2.CHAIN_APPROX_SIMPLE)


        if len(cnts) > 0:
                cnt = sorted(cnts, key = cv2.contourArea, reverse = True)[0]

                # compute the (rotated) bounding box around then
                # contour and then draw it		
                rect = np.int32(cv2.boxPoints(cv2.minAreaRect(cnt)))
                cv2.drawContours(image, [rect], -1, (0, 255, 0), 2)

                i = rect[0]
                j = rect[1]
                k = rect[2]
                w = rect[3]


                x = (i[0] + j[0] + k[0] + w[0])/4
                y = (i[1] + j[1] + k[1] + w[1])/4
                

                if x < 480:
                        c.send("0")
                else:
                        c.send("1")
                     
                #strd = str(i)
                #c.send(strd)
                #strd = str(j)
                #c.send(strd)
                #strd = str(k)
                #c.send(strd)
                #strd = str(w)
                #c.send(strd)
                c.send("\n")

	
	# show the frame
	cv2.imshow("Frame", image)
	key = cv2.waitKey(1) & 0xFF
 
	# clear the stream in preparation for the next frame
	rawCapture.truncate(0)
 
	# if the `q` key was pressed, break from the loop
	if key == ord("q"):
		break
