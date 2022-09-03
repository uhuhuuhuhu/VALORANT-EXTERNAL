import pyautogui

color = (255, 151, 255)

s = pyautogui.screenshot()
for y in range(s.height):
  if s.getpixel((x, y)) == color:
    print(x, y)
