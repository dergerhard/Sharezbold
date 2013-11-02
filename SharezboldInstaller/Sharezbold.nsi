############################################################################################
#      NSIS Installation Script created by NSIS Quick Setup Script Generator v1.09.18
#               Entirely Edited with NullSoft Scriptable Installation System                
#              by Vlasis K. Barkas aka Red Wine red_wine@freemail.gr Sep 2006               
############################################################################################

!define APP_NAME "Sharezbold"
!define COMP_NAME "FHWN"
!define WEB_SITE "http://www.fhwn.ac.at"
!define VERSION "00.00.00.01"
!define COPYRIGHT "Liebmann/Holzgethan"
!define DESCRIPTION "Application"
!define INSTALLER_NAME ".\bin\setup.exe"
!define MAIN_APP_EXE "Sharezbold.exe"
!define INSTALL_TYPE "SetShellVarContext all"
!define REG_ROOT "HKLM"
!define REG_APP_PATH "Software\Microsoft\Windows\CurrentVersion\App Paths\${MAIN_APP_EXE}"
!define UNINSTALL_PATH "Software\Microsoft\Windows\CurrentVersion\Uninstall\${APP_NAME}"

######################################################################

VIProductVersion  "${VERSION}"
VIAddVersionKey "ProductName"  "${APP_NAME}"
VIAddVersionKey "CompanyName"  "${COMP_NAME}"
VIAddVersionKey "LegalCopyright"  "${COPYRIGHT}"
VIAddVersionKey "FileDescription"  "${DESCRIPTION}"
VIAddVersionKey "FileVersion"  "${VERSION}"

######################################################################

SetCompressor ZLIB
Name "${APP_NAME}"
Caption "${APP_NAME}"
OutFile "${INSTALLER_NAME}"
BrandingText "${APP_NAME}"
XPStyle on
InstallDirRegKey "${REG_ROOT}" "${REG_APP_PATH}" ""
InstallDir "$PROGRAMFILES\Sharezbold"

######################################################################

!include "MUI.nsh"

!define MUI_ABORTWARNING
!define MUI_UNABORTWARNING

!define MUI_LANGDLL_REGISTRY_ROOT "${REG_ROOT}"
!define MUI_LANGDLL_REGISTRY_KEY "${UNINSTALL_PATH}"
!define MUI_LANGDLL_REGISTRY_VALUENAME "Installer Language"

!insertmacro MUI_PAGE_WELCOME

!ifdef LICENSE_TXT
!insertmacro MUI_PAGE_LICENSE "${LICENSE_TXT}"
!endif

!insertmacro MUI_PAGE_DIRECTORY

!ifdef REG_START_MENU
!define MUI_STARTMENUPAGE_NODISABLE
!define MUI_STARTMENUPAGE_DEFAULTFOLDER "Sharezbold"
!define MUI_STARTMENUPAGE_REGISTRY_ROOT "${REG_ROOT}"
!define MUI_STARTMENUPAGE_REGISTRY_KEY "${UNINSTALL_PATH}"
!define MUI_STARTMENUPAGE_REGISTRY_VALUENAME "${REG_START_MENU}"
!insertmacro MUI_PAGE_STARTMENU Application $SM_Folder
!endif

!insertmacro MUI_PAGE_INSTFILES

!define MUI_FINISHPAGE_RUN "$INSTDIR\${MAIN_APP_EXE}"
!insertmacro MUI_PAGE_FINISH

!insertmacro MUI_UNPAGE_CONFIRM

!insertmacro MUI_UNPAGE_INSTFILES

!insertmacro MUI_UNPAGE_FINISH

!insertmacro MUI_LANGUAGE "Czech"
!insertmacro MUI_LANGUAGE "English"
!insertmacro MUI_LANGUAGE "German"
!insertmacro MUI_LANGUAGE "Romanian"

!insertmacro MUI_RESERVEFILE_LANGDLL

######################################################################

Function .onInit
!insertmacro MUI_LANGDLL_DISPLAY
FunctionEnd

######################################################################

Section -MainProgram
${INSTALL_TYPE}
SetOverwrite ifnewer
SetOutPath "$INSTDIR"
File "..\Sharezbold\bin\Release\FileMigration.dll"
File "..\Sharezbold\bin\Release\FileMigration.pdb"
File "..\Sharezbold\bin\Release\FileMigration.XmlSerializers.dll"
File "..\Sharezbold\bin\Release\Microsoft.SharePoint.Client.dll"
File "..\Sharezbold\bin\Release\Microsoft.SharePoint.Client.Runtime.dll"
File "..\Sharezbold\bin\Release\Sharezbold.ContentMigration.dll"
File "..\Sharezbold\bin\Release\Sharezbold.ContentMigration.pdb"
File "..\Sharezbold\bin\Release\Sharezbold.ContentMigration.XmlSerializers.dll"
File "..\Sharezbold\bin\Release\Sharezbold.exe"
File "..\Sharezbold\bin\Release\Sharezbold.exe.config"
File "..\Sharezbold\bin\Release\Sharezbold.pdb"
File "..\Sharezbold\bin\Release\Sharezbold.vshost.exe"
File "..\Sharezbold\bin\Release\Sharezbold.vshost.exe.config"
File "..\Sharezbold\bin\Release\Sharezbold.vshost.exe.manifest"
File "..\Sharezbold\bin\Release\TypeMigration.dll"
File "..\Sharezbold\bin\Release\TypeMigration.pdb"
File "..\Sharezbold\bin\Release\TypeMigration.XmlSerializers.dll"
SectionEnd

######################################################################

Section -Icons_Reg
SetOutPath "$INSTDIR"
WriteUninstaller "$INSTDIR\uninstall.exe"

!ifdef REG_START_MENU
!insertmacro MUI_STARTMENU_WRITE_BEGIN Application
CreateDirectory "$SMPROGRAMS\$SM_Folder"
CreateShortCut "$SMPROGRAMS\$SM_Folder\${APP_NAME}.lnk" "$INSTDIR\${MAIN_APP_EXE}"
CreateShortCut "$SMPROGRAMS\$SM_Folder\Uninstall ${APP_NAME}.lnk" "$INSTDIR\uninstall.exe"

!ifdef WEB_SITE
WriteIniStr "$INSTDIR\${APP_NAME} website.url" "InternetShortcut" "URL" "${WEB_SITE}"
CreateShortCut "$SMPROGRAMS\$SM_Folder\${APP_NAME} Website.lnk" "$INSTDIR\${APP_NAME} website.url"
!endif
!insertmacro MUI_STARTMENU_WRITE_END
!endif

!ifndef REG_START_MENU
CreateDirectory "$SMPROGRAMS\Sharezbold"
CreateShortCut "$SMPROGRAMS\Sharezbold\${APP_NAME}.lnk" "$INSTDIR\${MAIN_APP_EXE}"
CreateShortCut "$SMPROGRAMS\Sharezbold\Uninstall ${APP_NAME}.lnk" "$INSTDIR\uninstall.exe"

!ifdef WEB_SITE
WriteIniStr "$INSTDIR\${APP_NAME} website.url" "InternetShortcut" "URL" "${WEB_SITE}"
CreateShortCut "$SMPROGRAMS\Sharezbold\${APP_NAME} Website.lnk" "$INSTDIR\${APP_NAME} website.url"
!endif
!endif

WriteRegStr ${REG_ROOT} "${REG_APP_PATH}" "" "$INSTDIR\${MAIN_APP_EXE}"
WriteRegStr ${REG_ROOT} "${UNINSTALL_PATH}"  "DisplayName" "${APP_NAME}"
WriteRegStr ${REG_ROOT} "${UNINSTALL_PATH}"  "UninstallString" "$INSTDIR\uninstall.exe"
WriteRegStr ${REG_ROOT} "${UNINSTALL_PATH}"  "DisplayIcon" "$INSTDIR\${MAIN_APP_EXE}"
WriteRegStr ${REG_ROOT} "${UNINSTALL_PATH}"  "DisplayVersion" "${VERSION}"
WriteRegStr ${REG_ROOT} "${UNINSTALL_PATH}"  "Publisher" "${COMP_NAME}"

!ifdef WEB_SITE
WriteRegStr ${REG_ROOT} "${UNINSTALL_PATH}"  "URLInfoAbout" "${WEB_SITE}"
!endif
SectionEnd

######################################################################

Section Uninstall
${INSTALL_TYPE}
Delete "$INSTDIR\FileMigration.dll"
Delete "$INSTDIR\FileMigration.pdb"
Delete "$INSTDIR\FileMigration.XmlSerializers.dll"
Delete "$INSTDIR\Microsoft.SharePoint.Client.dll"
Delete "$INSTDIR\Microsoft.SharePoint.Client.Runtime.dll"
Delete "$INSTDIR\Sharezbold.ContentMigration.dll"
Delete "$INSTDIR\Sharezbold.ContentMigration.pdb"
Delete "$INSTDIR\Sharezbold.ContentMigration.XmlSerializers.dll"
Delete "$INSTDIR\Sharezbold.exe"
Delete "$INSTDIR\Sharezbold.exe.config"
Delete "$INSTDIR\Sharezbold.pdb"
Delete "$INSTDIR\Sharezbold.vshost.exe"
Delete "$INSTDIR\Sharezbold.vshost.exe.config"
Delete "$INSTDIR\Sharezbold.vshost.exe.manifest"
Delete "$INSTDIR\TypeMigration.dll"
Delete "$INSTDIR\TypeMigration.pdb"
Delete "$INSTDIR\TypeMigration.XmlSerializers.dll"
Delete "$INSTDIR\uninstall.exe"
!ifdef WEB_SITE
Delete "$INSTDIR\${APP_NAME} website.url"
!endif

RmDir "$INSTDIR"

!ifdef REG_START_MENU
!insertmacro MUI_STARTMENU_GETFOLDER "Application" $SM_Folder
Delete "$SMPROGRAMS\$SM_Folder\${APP_NAME}.lnk"
Delete "$SMPROGRAMS\$SM_Folder\Uninstall ${APP_NAME}.lnk"
!ifdef WEB_SITE
Delete "$SMPROGRAMS\$SM_Folder\${APP_NAME} Website.lnk"
!endif
RmDir "$SMPROGRAMS\$SM_Folder"
!endif

!ifndef REG_START_MENU
Delete "$SMPROGRAMS\Sharezbold\${APP_NAME}.lnk"
Delete "$SMPROGRAMS\Sharezbold\Uninstall ${APP_NAME}.lnk"
!ifdef WEB_SITE
Delete "$SMPROGRAMS\Sharezbold\${APP_NAME} Website.lnk"
!endif
RmDir "$SMPROGRAMS\Sharezbold"
!endif

DeleteRegKey ${REG_ROOT} "${REG_APP_PATH}"
DeleteRegKey ${REG_ROOT} "${UNINSTALL_PATH}"
SectionEnd

######################################################################

Function un.onInit
!insertmacro MUI_UNGETLANGUAGE
FunctionEnd

######################################################################

