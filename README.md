# Raintoaster
An addon to toast notifications for [Rainmeter desktop customization tool](https://www.rainmeter.net).

# Usage
This is an app you can use with RunCommand plugin to send toast notifications to users.

# Parameters
### Program Path:
- This is the program toast notification will execute when interacted with by user. The default is the default path to Rainmeter, e.g. ProgramFiles\Rainmeter\Rainmeter.exe.
- If you are distributing your skin, it is highly recommended that you use `#PROGRAMPATH#Rainmeter.exe` as program path.
- It can be other than Rainmeter but make sure you pass parameters.
- Syntax: `--programpath "<absolutepath>"`
- Example:
```
--programpath "#PROGRAMPATH#Rainmeter.exe"
```

### Parameter:
- This the argument that will be passed down to the program. Default is `[!Log \"Toasted from Raintoast!\"]`
- Make sure you provide a parameter if your `programpath` is different than Rainmeter.
- Parameter: `--`
### Title:
- This will be the main title of your toast. 
- Syntax: `--title "<string>"`
- Example: 
```
--title "ExampleSkin"
```

### Adaptive Text:
- This will be the descriptive text under title. There can be two adaptive texts, each of max 2 lines.
- Syntax: `--adaptivetext1 "<string>"`, `--adaptivetext2 "<string>"`
- Example: 
```
--adaptivetext1 "Update available for ExampleSkin!" --adaptivetext2 "Click Update to install or get the latest version from our website."
```
### Logo:
- This will be the logo shown in the toast.
- The logo image must be kept in the directory where the app is. E.g. if the app is in @Resources\Toaster, image can be in @Resources\Toaster\Assets.
- Syntax: `--logo "<relativepath>"`
- Example: 
```
--logo "Assets\myToastLogo.png"
```
### HeroImage:
- This will appear on the top of toast notification, like a header image. Path must be relative. See Logo #2.
- Syntax: `--heroimage "<relativepath>"`
- Example: 
```
--heroimage "Assets\myHeroImage.png"
```
### Inline Image:
- This will appear as a big image embedded in the toast notification.
- Syntax: `--inlineimage "<relativepath>"`
- Example: 
```
--inlineimage "Assets\myInlineImage.png"
```
### Attribution Text:
- This will appear as a small line of text under adaptive text.
- Syntax: `--attribution "<string>"`
- Example: 
```
--attribution "With ❤️, from Example community"
```
### Buttons:
- Well, buttons.
- Syntax: `--button1 "<argument>"`, `--button2 "<argument>"`, `--button3 "<argument>"`, `--button4 "<argument>"`, `--button5 "<argument>"`
- Argument Format: `buttonName|bangs`
- Example: 
```
--button1 "Update|[!CommandMeasure PowershellRM \"DownloadAndUpdate\" \"#CURRENTCONFIG#\"]" --button2 "Website|[\"https://www.myexamplewebsite.com\"]"
```
- > ⚠️ While working with input boxes buttons will have a different argument format. Discussed in `Input Box`.
### Input Box:
- Can be used to get text input from user. New lines will be replaced by `#CRLF#`.
- Parameter: `--inputbox`
- Buttons: While using input box, atleast one button needs to be set up to handle the input box data. This is done by adding `input=true&` to the start of bangs. 
  
  E.g. `--buttonN "ButtonName|input=true&[bangs]"`
- Formatting Bangs: Now in the bangs, you can use `$input$` temporary variable to get the input from user. This will be replaced by what user input.

  E.g. `[!SomeBang \"Some$input$Parameter\"]`. Always remember to put quotes around them as user input may contain whitespace.
  
- Example: 
```
--inputbox "Enter image path" --button1 "Show|input=true&[!SetOption MeterImage ImageName \"$input$\" \"#CURRENTCONFIG#\"][!UpdateMeter MeterImage \"#CURRENTCONFIG#\"][!Redraw \"#CURRENTCONFIG#\"]"
```
