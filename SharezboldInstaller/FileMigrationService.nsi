############################################################################################
#      NSIS Installation Script created by NSIS Quick Setup Script Generator v1.09.18
#               Entirely Edited with NullSoft Scriptable Installation System                
#              by Vlasis K. Barkas aka Red Wine red_wine@freemail.gr Sep 2006               
############################################################################################

!define APP_NAME "FileMigrationService"
!define COMP_NAME "FH Wiener Neustadt"
!define WEB_SITE "http://www.fhwn.ac.at"
!define VERSION "00.00.00.01"
!define COPYRIGHT "FHWN © 2013"
!define DESCRIPTION "Service for Sharezbold"
!define INSTALLER_NAME ".\bin\FileMigrationServiceSetup.exe"
!define MAIN_APP_EXE "FileMigrationHostApplication.exe"
!define INSTALL_TYPE "SetShellVarContext all"
!define REG_ROOT "HKLM"
!define REG_APP_PATH "Software\Microsoft\Windows\CurrentVersion\App Paths\${MAIN_APP_EXE}"
!define UNINSTALL_PATH "Software\Microsoft\Windows\CurrentVersion\Uninstall\${APP_NAME}"

!define REG_START_MENU "Start Menu Folder"

var SM_Folder

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
InstallDir "$PROGRAMFILES\FileMigrationService"

######################################################################

!include "MUI.nsh"

!define MUI_ABORTWARNING
!define MUI_UNABORTWARNING

!insertmacro MUI_PAGE_WELCOME

!ifdef LICENSE_TXT
!insertmacro MUI_PAGE_LICENSE "${LICENSE_TXT}"
!endif

!ifdef REG_START_MENU
!define MUI_STARTMENUPAGE_DEFAULTFOLDER "FileMigrationService"
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

!insertmacro MUI_LANGUAGE "English"

######################################################################

Section -MainProgram
${INSTALL_TYPE}
SetOverwrite ifnewer
SetOutPath "$INSTDIR"
File "..\FileMigrationHostApplication\bin\x64\Release\FileMigrationContract.dll"
File "..\FileMigrationHostApplication\bin\x64\Release\FileMigrationContract.pdb"
File "..\FileMigrationHostApplication\bin\x64\Release\FileMigrationHostApplication.exe"
File "..\FileMigrationHostApplication\bin\x64\Release\FileMigrationHostApplication.exe.config"
File "..\FileMigrationHostApplication\bin\x64\Release\FileMigrationHostApplication.pdb"
File "..\FileMigrationHostApplication\bin\x64\Release\FileMigrationHostApplication.vshost.exe"
File "..\FileMigrationHostApplication\bin\x64\Release\FileMigrationHostApplication.vshost.exe.config"
File "..\FileMigrationHostApplication\bin\x64\Release\FileMigrationHostApplication.vshost.exe.manifest"
File "..\FileMigrationHostApplication\bin\x64\Release\FileMigrationServiceImplementation.dll"
File "..\FileMigrationHostApplication\bin\x64\Release\FileMigrationServiceImplementation.pdb"
File "..\FileMigrationHostApplication\bin\x64\Release\Microsoft.SharePoint.dll"
SectionEnd

######################################################################

Section -Icons_Reg
SetOutPath "$INSTDIR"
WriteUninstaller "$INSTDIR\uninstall.exe"

!ifdef REG_START_MENU
!insertmacro MUI_STARTMENU_WRITE_BEGIN Application
CreateDirectory "$SMPROGRAMS\$SM_Folder"
CreateShortCut "$SMPROGRAMS\$SM_Folder\${APP_NAME}.lnk" "$INSTDIR\${MAIN_APP_EXE}"
!ifdef WEB_SITE
WriteIniStr "$INSTDIR\${APP_NAME} website.url" "InternetShortcut" "URL" "${WEB_SITE}"
CreateShortCut "$SMPROGRAMS\$SM_Folder\${APP_NAME} Website.lnk" "$INSTDIR\${APP_NAME} website.url"
!endif
!insertmacro MUI_STARTMENU_WRITE_END
!endif

!ifndef REG_START_MENU
CreateDirectory "$SMPROGRAMS\FileMigrationService"
CreateShortCut "$SMPROGRAMS\FileMigrationService\${APP_NAME}.lnk" "$INSTDIR\${MAIN_APP_EXE}"
!ifdef WEB_SITE
WriteIniStr "$INSTDIR\${APP_NAME} website.url" "InternetShortcut" "URL" "${WEB_SITE}"
CreateShortCut "$SMPROGRAMS\FileMigrationService\${APP_NAME} Website.lnk" "$INSTDIR\${APP_NAME} website.url"
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
Delete "$INSTDIR\FileMigrationContract.dll"
Delete "$INSTDIR\FileMigrationContract.pdb"
Delete "$INSTDIR\FileMigrationHostApplication.exe"
Delete "$INSTDIR\FileMigrationHostApplication.exe.config"
Delete "$INSTDIR\FileMigrationHostApplication.pdb"
Delete "$INSTDIR\FileMigrationHostApplication.vshost.exe"
Delete "$INSTDIR\FileMigrationHostApplication.vshost.exe.config"
Delete "$INSTDIR\FileMigrationHostApplication.vshost.exe.manifest"
Delete "$INSTDIR\FileMigrationServiceImplementation.dll"
Delete "$INSTDIR\FileMigrationServiceImplementation.pdb"
Delete "$INSTDIR\Microsoft.SharePoint.dll"
Delete "$INSTDIR\uninstall.exe"
!ifdef WEB_SITE
Delete "$INSTDIR\${APP_NAME} website.url"
!endif

RmDir "$INSTDIR"

!ifdef REG_START_MENU
!insertmacro MUI_STARTMENU_GETFOLDER "Application" $SM_Folder
Delete "$SMPROGRAMS\$SM_Folder\${APP_NAME}.lnk"
!ifdef WEB_SITE
Delete "$SMPROGRAMS\$SM_Folder\${APP_NAME} Website.lnk"
!endif
RmDir "$SMPROGRAMS\$SM_Folder"
!endif

!ifndef REG_START_MENU
Delete "$SMPROGRAMS\FileMigrationService\${APP_NAME}.lnk"
!ifdef WEB_SITE
Delete "$SMPROGRAMS\FileMigrationService\${APP_NAME} Website.lnk"
!endif
RmDir "$SMPROGRAMS\FileMigrationService"
!endif

DeleteRegKey ${REG_ROOT} "${REG_APP_PATH}"
DeleteRegKey ${REG_ROOT} "${UNINSTALL_PATH}"
SectionEnd

######################################################################

