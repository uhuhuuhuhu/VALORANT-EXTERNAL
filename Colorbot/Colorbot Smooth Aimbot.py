import pyautogui

color = (255, 151, 255)

s = pyautogui.screenshot()
for x in range(s.width):
	for y in range(s.height):
  		if s.getpixel((x, y)) == color: # if enemy outline color found
    			pyautogui.moveTo(x, y, 0.2)
                        pyautogui.click()
