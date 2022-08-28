import pyautogui

# enemy outline color purple (can be improved - feel free to submit merge requests, etc or contact me on Discord: Alune#9045)
color = (255, 151, 255)

# creates a screenshot to look for outlines
s = pyautogui.screenshot()
for x in range(s.width):
	for y in range(s.height):
  		if s.getpixel((x, y)) == color: # if enemy outline color found
    			print(x, y) # print position
          
          # uncomment the 2 lines below this for smooth movement
          # pyautogui.moveTo(x, y, 0.2)
          # pyautogui.click()
          
          # unsilent cbot - makes it pretty obvious
          pyautogui.click(x, y)
