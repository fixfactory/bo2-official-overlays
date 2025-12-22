; Define the installer name and output
!define PRODUCT_NAME "benofficial2's Official Overlays"
Name "${PRODUCT_NAME}"
!define PRODUCT_VERSION "5.0 (beta1)"
!define PRODUCT_PUBLISHER "benofficial2"
!define PRODUCT_WEB_SITE "https://twitch.tv/benofficial2"
!define PRODUCT_DIR_REGKEY "Software\bo2-official-overlays"
!define SIMHUB_VERSION "9.10.15"

; Include Modern User Interface
!include "MUI2.nsh"
!include "InstallOptions.nsh"

; Must have NsProcess plugin v1.6 installed (https://nsis.sourceforge.io/NsProcess_plugin)
!include "nsProcess.nsh"

; MUI Settings
!define MUI_ABORTWARNING
;!define MUI_ICON "installer.ico" ; Optional: Add your icon file here

; Custom Welcome Page Text
!define MUI_WELCOMEPAGE_TITLE "Welcome to ${PRODUCT_NAME} ${PRODUCT_VERSION} Setup"
!define MUI_WELCOMEPAGE_TEXT "You will soon be racing with the greatest overlays in sim-racing history.$\r$\n$\r$\nSince the overlays are made with SimHub, it is necessary to have installed SimHub version ${SIMHUB_VERSION} or later.$\r$\n$\r$\nThis Setup will guide you through this process.$\r$\n$\r$\nClick Next to continue."

; Custom Finish Page Text
!define MUI_FINISHPAGE_TITLE "Completing ${PRODUCT_NAME} ${PRODUCT_VERSION} Setup"
!define MUI_FINISHPAGE_TEXT "Thank you for installing my overlays. I hope you will enjoy them!$\r$\n$\r$\nPlease consider following me on Twitch at twitch.tv/benofficial2$\r$\n$\r$\nClick Finish to close Setup and open Twitch."

; Pages
!insertmacro MUI_PAGE_WELCOME
!insertmacro MUI_PAGE_LICENSE "..\LICENSE"
Page Custom DependenciesPage DependenciesPageLeave
!insertmacro MUI_PAGE_DIRECTORY
!insertmacro MUI_PAGE_COMPONENTS
!insertmacro MUI_PAGE_INSTFILES
!define MUI_PAGE_CUSTOMFUNCTION_LEAVE FinishPageLeave
!insertmacro MUI_PAGE_FINISH

; Language Selection
!insertmacro MUI_LANGUAGE "English"

Function .onInit
    ${nsProcess::FindProcess} "SimHubWPF.exe" $R0
    ${If} $R0 == "0"
        MessageBox MB_ICONEXCLAMATION|MB_RETRYCANCEL "SimHub is running. Please close it before continuing." IDRETRY retry
        Quit
        retry:
        Call .onInit ; Retry the check
    ${EndIf}
FunctionEnd

Function DependenciesPage
  ReserveFile "DependenciesPage.ini"
  !insertmacro MUI_HEADER_TEXT "Important Pre-requisites" "You need to install SimHub before continuing if you don't have it already"
  !insertmacro INSTALLOPTIONS_EXTRACT "DependenciesPage.ini"
  !insertmacro INSTALLOPTIONS_DISPLAY "DependenciesPage.ini"
FunctionEnd
 
Function DependenciesPageLeave
  # Find out which field event called us. 0 = Next button called us.
  !insertmacro INSTALLOPTIONS_READ $R0 "DependenciesPage.ini" "Settings" "State"
  ${If} $R0 == 2 # Field 2.
    ExecShell "open" "https://www.simhubdash.com/download-2/"
    abort
  ${EndIf}
FunctionEnd

; Installer attributes
OutFile "..\Bin\bo2-official-overlays-install-v${PRODUCT_VERSION}.exe"
InstallDir "$PROGRAMFILES\SimHub"
DirText "Setup will install ${PRODUCT_NAME} in the following folder.$\r$\nIMPORTANT: Choose the folder where SimHub is installed on your computer."
ShowInstDetails show
ShowUninstDetails show

# Use SimHub's install directory from the Registry if present. Fallsback to InstallDir.
InstallDirRegKey HKCU "SOFTWARE\SimHub" InstallDirectory

; Installer Sections
Section "Dash" SEC_FOLDER1
  SetOutPath "$INSTDIR\DashTemplates\benofficial2 - iRacing Dash"
  File /r "..\Overlays\benofficial2 - iRacing Dash\*.*"
SectionEnd

Section "Delta" SEC_FOLDER2
  SetOutPath "$INSTDIR\DashTemplates\benofficial2 - iRacing Delta"
  File /r "..\Overlays\benofficial2 - iRacing Delta\*.*"
SectionEnd

Section "Inputs" SEC_FOLDER3
  SetOutPath "$INSTDIR\DashTemplates\benofficial2 - iRacing Inputs"
  File /r "..\Overlays\benofficial2 - iRacing Inputs\*.*"
SectionEnd

Section "Relative" SEC_FOLDER4
  SetOutPath "$INSTDIR\DashTemplates\benofficial2 - iRacing Relative"
  File /r "..\Overlays\benofficial2 - iRacing Relative\*.*"
SectionEnd

Section "Standings" SEC_FOLDER5
  SetOutPath "$INSTDIR\DashTemplates\benofficial2 - iRacing Standings"
  File /r "..\Overlays\benofficial2 - iRacing Standings\*.*"
SectionEnd

Section "Track Map" SEC_FOLDER6
  SetOutPath "$INSTDIR\DashTemplates\benofficial2 - iRacing Track Map"
  File /r "..\Overlays\benofficial2 - iRacing Track Map\*.*"
SectionEnd

Section "Setup Cover" SEC_FOLDER7
  SetOutPath "$INSTDIR\DashTemplates\benofficial2 - iRacing Setup Cover"
  File /r "..\Overlays\benofficial2 - iRacing Setup Cover\*.*"
SectionEnd

Section "Twitch Chat" SEC_FOLDER8
  SetOutPath "$INSTDIR\DashTemplates\benofficial2 - Twitch Chat"
  File /r "..\Overlays\benofficial2 - Twitch Chat\*.*"
SectionEnd

Section "Launch Assist" SEC_FOLDER9
  SetOutPath "$INSTDIR\DashTemplates\benofficial2 - iRacing Launch Assist"
  File /r "..\Overlays\benofficial2 - iRacing Launch Assist\*.*"
SectionEnd

Section "Spotter" SEC_FOLDER10
  SetOutPath "$INSTDIR\DashTemplates\benofficial2 - iRacing Spotter"
  File /r "..\Overlays\benofficial2 - iRacing Spotter\*.*"
SectionEnd

Section "Setup Fuel Calculator" SEC_FOLDER11
  SetOutPath "$INSTDIR\DashTemplates\benofficial2 - iRacing Fuel Calculator"
  File /r "..\Overlays\benofficial2 - iRacing Fuel Calculator\*.*"
SectionEnd

Section "Wind" SEC_FOLDER12
  SetOutPath "$INSTDIR\DashTemplates\benofficial2 - iRacing Wind"
  File /r "..\Overlays\benofficial2 - iRacing Wind\*.*"
SectionEnd

Section "Multi-Class Standings" SEC_FOLDER13
  SetOutPath "$INSTDIR\DashTemplates\benofficial2 - iRacing Multi-Class Standings"
  File /r "..\Overlays\benofficial2 - iRacing Multi-Class Standings\*.*"
SectionEnd

Section "Precipitation" SEC_FOLDER14
  SetOutPath "$INSTDIR\DashTemplates\benofficial2 - iRacing Precipitation"
  File /r "..\Overlays\benofficial2 - iRacing Precipitation\*.*"
SectionEnd

Section "Track Wetness" SEC_FOLDER15
  SetOutPath "$INSTDIR\DashTemplates\benofficial2 - iRacing Track Wetness"
  File /r "..\Overlays\benofficial2 - iRacing Track Wetness\*.*"
SectionEnd

Section "Highlighted Driver" SEC_FOLDER16
  SetOutPath "$INSTDIR\DashTemplates\benofficial2 - iRacing Highlighted Driver"
  File /r "..\Overlays\benofficial2 - iRacing Highlighted Driver\*.*"
SectionEnd

Section "Fuel Calculator" SEC_FOLDER17
  SetOutPath "$INSTDIR\DashTemplates\benofficial2 - iRacing Fuel Calculator Live"
  File /r "..\Overlays\benofficial2 - iRacing Fuel Calculator Live\*.*"
SectionEnd

Section "Plugin" SEC_PLUGIN
  SectionIn RO
  WriteUninstaller "$INSTDIR\bo2-official-overlays-uninstall.exe"

  SetOutPath "$INSTDIR"
  File /oname=bo2-official-overlays-license.txt "..\LICENSE"
  File "..\Plugin\bin\Release\benofficial2.Plugin.dll"

  SetOutPath "$INSTDIR\ImageLibrary\benofficial2\CarLogos"
  File /r "..\Images\CarLogos\*.*"

  SetOutPath "$INSTDIR\ImageLibrary\benofficial2\Flairs"
  File /r "..\Images\Flairs\*.*"

  SetOutPath "$INSTDIR\ImageLibrary\benofficial2\Icons"
  File /r "..\Images\Icons\*.*"

  ; Register uninstaller
  WriteRegStr HKLM "Software\Microsoft\Windows\CurrentVersion\Uninstall\bo2-official-overlays" \
                "DisplayName" "benofficial2's Official Overlays"
  WriteRegStr HKLM "Software\Microsoft\Windows\CurrentVersion\Uninstall\bo2-official-overlays" \
                "UninstallString" "$\"$INSTDIR\bo2-official-overlays-uninstall.exe$\""
  WriteRegStr HKLM "Software\Microsoft\Windows\CurrentVersion\Uninstall\bo2-official-overlays" \
                "DisplayVersion" "${PRODUCT_VERSION}"
SectionEnd

; Components Page Descriptions
!insertmacro MUI_FUNCTION_DESCRIPTION_BEGIN
  !insertmacro MUI_DESCRIPTION_TEXT ${SEC_FOLDER1} "Install iRacing Dash overlay"
  !insertmacro MUI_DESCRIPTION_TEXT ${SEC_FOLDER2} "Install iRacing Delta overlay"
  !insertmacro MUI_DESCRIPTION_TEXT ${SEC_FOLDER3} "Install iRacing Inputs overlay"
  !insertmacro MUI_DESCRIPTION_TEXT ${SEC_FOLDER4} "Install iRacing Relative overlay"
  !insertmacro MUI_DESCRIPTION_TEXT ${SEC_FOLDER5} "Install iRacing Standings overlay"
  !insertmacro MUI_DESCRIPTION_TEXT ${SEC_FOLDER6} "Install iRacing Track Map overlay"
  !insertmacro MUI_DESCRIPTION_TEXT ${SEC_FOLDER7} "Install iRacing Setup Cover overlay"
  !insertmacro MUI_DESCRIPTION_TEXT ${SEC_FOLDER8} "Install Twitch Chat overlay"
  !insertmacro MUI_DESCRIPTION_TEXT ${SEC_FOLDER9} "Install iRacing Launch Assist overlay"
  !insertmacro MUI_DESCRIPTION_TEXT ${SEC_FOLDER10} "Install iRacing Spotter overlay"
  !insertmacro MUI_DESCRIPTION_TEXT ${SEC_FOLDER11} "Install iRacing Setup Fuel Calculator overlay"
  !insertmacro MUI_DESCRIPTION_TEXT ${SEC_FOLDER12} "Install iRacing Wind overlay"
  !insertmacro MUI_DESCRIPTION_TEXT ${SEC_FOLDER13} "Install iRacing Multi-Class Standings overlay"
  !insertmacro MUI_DESCRIPTION_TEXT ${SEC_FOLDER14} "Install iRacing Precipitation overlay"
  !insertmacro MUI_DESCRIPTION_TEXT ${SEC_FOLDER15} "Install iRacing Track Wetness overlay"
  !insertmacro MUI_DESCRIPTION_TEXT ${SEC_FOLDER16} "Install iRacing Highlighted Driver overlay"
  !insertmacro MUI_DESCRIPTION_TEXT ${SEC_FOLDER17} "Install iRacing Fuel Calculator overlay"
  !insertmacro MUI_DESCRIPTION_TEXT ${SEC_PLUGIN} "Install necessary files such as the plugin, license and uninstaller"
!insertmacro MUI_FUNCTION_DESCRIPTION_END

; Uninstaller Section
Section "Uninstall"
  RMDir /r "$INSTDIR\DashTemplates\benofficial2 - iRacing Dash"
  RMDir /r "$INSTDIR\DashTemplates\benofficial2 - iRacing Delta"
  RMDir /r "$INSTDIR\DashTemplates\benofficial2 - iRacing Inputs"
  RMDir /r "$INSTDIR\DashTemplates\benofficial2 - iRacing Relative"
  RMDir /r "$INSTDIR\DashTemplates\benofficial2 - iRacing Standings"
  RMDir /r "$INSTDIR\DashTemplates\benofficial2 - iRacing Track Map"
  RMDir /r "$INSTDIR\DashTemplates\benofficial2 - iRacing Setup Cover"
  RMDir /r "$INSTDIR\DashTemplates\benofficial2 - Twitch Chat"
  RMDir /r "$INSTDIR\DashTemplates\benofficial2 - iRacing Launch Assist"
  RMDir /r "$INSTDIR\DashTemplates\benofficial2 - iRacing Spotter"
  RMDir /r "$INSTDIR\DashTemplates\benofficial2 - iRacing Fuel Calculator"
  RMDir /r "$INSTDIR\DashTemplates\benofficial2 - iRacing Wind"
  RMDir /r "$INSTDIR\DashTemplates\benofficial2 - iRacing Multi-Class Standings"
  RMDir /r "$INSTDIR\DashTemplates\benofficial2 - iRacing Precipitation"
  RMDir /r "$INSTDIR\DashTemplates\benofficial2 - iRacing Track Wetness"
  RMDir /r "$INSTDIR\DashTemplates\benofficial2 - iRacing Highlighted Driver"
  RMDir /r "$INSTDIR\DashTemplates\benofficial2 - iRacing Fuel Calculator Live"
  RMDir /r "$INSTDIR\ImageLibrary\benofficial2"
      
  ; Remove the uninstaller itself
  Delete "$INSTDIR\bo2-official-overlays-uninstall.exe"
  Delete "$INSTDIR\bo2-official-overlays-license.txt"
  Delete "$INSTDIR\benofficial2.Plugin.dll"

  DeleteRegKey /ifempty HKCU "${PRODUCT_DIR_REGKEY}"
  DeleteRegKey HKLM "Software\Microsoft\Windows\CurrentVersion\Uninstall\bo2-official-overlays"
SectionEnd

Function FinishPageLeave
  ExecShell "open" "https://www.twitch.tv/benofficial2"
FunctionEnd

; Sign executable using code-signing certificate
!finalize 'Sign.bat "%1"' ; %1 inside the batch file is the .exe to sign
!uninstfinalize 'Sign.bat "%1"'