# Raintoaster
An add-on to toast notifications for [Rainmeter desktop customization tool](https://www.rainmeter.net).

# Usage
This is an app you can use with RunCommand plugin to send toast notifications to users.

# Elements
Some important points before beginning:
```md
- All elements are rendered in the order they are defined.
- You must avoid unwanted spaces where delimiters are used, e.g.`|`
- You must put strings that contain whitespace characters inside double quotes, e.g. `"string with whitespace"`.
- You must escape double quotes that are present within the string, e.g. `"string \"with\" quotes"`
- All paths are relative.
```

### Program Path:
- This is the program toast notification will execute when interacted with by user. Default value is default path to Rainmeter, e.g. ProgramFiles\Rainmeter\Rainmeter.exe.
- If you are distributing your skin, it is highly recommended that you use `#PROGRAMPATH#Rainmeter.exe` as program path.
- It can be other than Rainmeter but make sure you pass valid parameters.
- Syntax: `--programpath "<absolutepath>"`
- Example:
```cmd
--programpath "#PROGRAMPATH#Rainmeter.exe"
```

### Parameter:
- This the argument that will be passed down to the program. Default is `[!Log \"Toasted from Raintoast!\"]`
- Make sure you provide a parameter if your `programpath` is different than Rainmeter.
- Syntax: `--parameter "<parameters>"`
- Example:
```cmd
--parameter "[!Log Hello][!Log \"I am death.crafter, creator of this skin.\"]"
```

### Adaptive Text:
- The first adaptive text is the title of your toast.
- Second and third will appear under the title.
- Each adaptive text can be of maximum two lines.
- Syntax: `--text "<string>"`
- Example: 
```cmd
--text "Title" --text "Info 1" --text "Info 2"
```

### Attribution Text:
- This will appear as a small line of text under adaptive text.
- Syntax: `--attribution "<string>"`
- Example: 
```cmd
--attribution "via MyExampleSkin"
```

### Logo:
- This will be the logo shown in the toast.
- The logo image must be kept in the directory where the app is. E.g. if the app is in @Resources\Toaster, image can be in @Resources\Toaster\Assets.
- Syntax: `--logo "<relativepath>"`
- Example: 
```cmd
--logo "Assets\myToastLogo.png"
```

### HeroImage:
- This will appear on the top of toast notification, like a header image. Path must be relative. See Logo #2.
- Syntax: `--heroimage "<relativepath>"`
- Example: 
```cmd
--heroimage "Assets\myHeroImage.png"
```

### Inline Image:
- This will appear as a big image embedded in the toast notification. Path must be relative. See Logo #2.
- Syntax: `--inlineimage "<relativepath>"`
- Example: 
```cmd
--inlineimage "Assets\myInlineImage.png"
```
### Buttons:
- Well, buttons.
- Syntax: `--button "<name>" "<parameter>"`
- Example: 
```cmd
--button "Okay" "[!Log Okay]" --button "Cancel" "[!Log Cancel]"
```

### Input Box:
- Get text input from user. New lines will be replaced by `#CRLF#`.
- Use `$<id>$` in button parameter to get the input.
- Syntax: `--inputbox "<id>" "<defaulttext>"`
- Example:
```cmd
--inputbox "name" "Enter your name" --button "Okay" "[!Log \"Your name is $name$\"]"
```

### Selection Box:
- User can select from given options. There can be a max of 5 options to choose from.
- Use `$<id>$` in button parameter to get selection.
- Syntax: `--selectionbox "<id>" "<option1value>|<option1text> <option2value>|<option2text>" ... `
- Options are defined as `&default;<value>|<text>` where,
  - `&default;`: use this at the start of default option. don't forget the semicolon.
  - `value`    : return value
  - `text`     : text shown as option to user
- Example:
```cmd
--selectionbox "theme" "dark|Dark" "light|Light" "&default;githubdark|GitHub Dark" "githublight|GitHub Light" "solarized|Solarized" --button "Okay" "[!Log \"$theme$ was chosen.\"]"
```
  - Github Dark will be the default option.

# Example
```ini
[Rainmeter]
Update = 1000
AccurateText = 1

[Variables]
Name =
AgeGroup =

;Notification content
Program = --programpath "#PROGRAMPATH#Rainmeter.exe"
Title = --text "MyExampleSkin"
Info = --text "This is a test run of the Raintoaster addon."
Info2 = --text "Please provide the requested info."
Logo = --logo "Assets\myLogo.png"
InputBox = --inputbox "name" "Enter your name"
SelectionBox = --selectionbox "agegroup" "Children|Below 14" "Youth|15-28" "Middle|29-48" "Old|Above49"
Button = --button "Okay" "[!SetVariable Name \"$name$\" \"#CURRENTCONFIG#\"][!SetVariable AgeGroup \"$agegroup$\" \"#CURRENTCONFIG#\"][!Update \"#CURRENTCONFIG#\"]"
Button2 = --button "Cancel" "[!Log \"No information provided\"]"

[Toaster]
Measure = Plugin
Plugin = RunCommand
Program = #@#Raintoaster.exe
Parameter = #Program# #Title# #Info# #Info2# #Logo# #InputBox# #SelectionBox# #Button# #Button2#

[MeterString]
Meter = String
FontFace = Segoe UI
FontSize = 20
AntiAlias = 1
SolidColor = FFFFFF
DynamicVariables = 1
Text = Name: #Name##CRLF#Age Group: #AgeGroup#
LeftMouseUpAction = [!CommandMeasure Toaster Run]
```
## Result


https://user-images.githubusercontent.com/77834863/135673411-96671181-6844-4b1c-85d6-128cf5ac5398.mp4

