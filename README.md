# Support Companion

<img width="256" alt="appicon" src="https://github.com/almenscorner/SupportCompanion/assets/78877636/159af7d0-df70-414b-9973-0ab5b6a068e4">


## Description

Support Companion is a macOS helper application, designed to empower end-users by providing them with
quick and easy access to crucial information and actions. This application is built to streamline a variety of
tasks, eliminating the need for extensive searching and complex navigation. Support Companion is equipped with a range
of features that enhance user productivity.

It integrates with Munki and Intune for application information and updates, and with
Intune for MDM information, providing a unified platform for managing these services. Users can view system information
such as macOS version, model, and serial number at a glance, and perform actions such as changing passwords, rebooting,
and more with just a few clicks.

This initial version relies on Munki and/or Intune for application information and updates, and Intune for some MDM actions. If you are
not using Munki or Intune, this app may not provide much information and actions at the moment.

If there are wishes to add other MDM specific actions and information, please let me know. I am open to
adding more MDM providers in the future if there is a demand for it.
I am only able to test with Intune, so if you have another MDM provider, I would appreciate your help in testing.

## Stage
The app is currently in `beta`, and I am looking for feedback on features, bugs, and improvements. Please let me know if
you have any suggestions or issues.

## Features

- **Actions**: Perform actions such as Change Password, Kill Intune MDM Agent, gather logs, reboot and more.
- **System Information**: Quickly view system information such as macOS version, model, serial number and last boot
  time.
- **Evergreen**: See which Munki catalogs the devices is a member of (requires a local device manifest with the SN as
  name).
- **Battery**: View battery information such as cycle count and health.
- **MDM**: View MDM information such as enrollment status and enrollment date.
- **Disk**: View disk information such as disk space and FileVault status.
- **Application Patching Progress**: View the progress of patching applications.
- **Pending Updates**: View pending updates for applications.
- **Applications**: View installed applications and their versions.
- **Identity**: View the current user's profile information and Kerberos SSO or Platform SSO information.

## Installation

For now this app is not signed as I do not have access to an Apple Developer Certificate.

1. Obtain the latest PKG installer from [releases](https://github.com/almenscorner/SupportCompanion/releases).
2. Download and install the MacAdmins Python package from [here](https://github.com/macadmins/python/releases/tag/v3.12.1.80742).
   - This is required for the app to run the scripts such as collecting MDM information.
3. Run the PKG installer.

## Installed files

The app is installed in the `/Applications/Utilities` folder and the following files and folders are installed:
- `/Applications/Utilities/SupportCompanion.app` - The app bundle
- `/Library/Application Support/SupportCompanion` - Folder containing the following files:
   - `Scripts` - Scripts used to get information such as MDM status
   - JSON files generated by the scripts for the app to read
- `/Library/LaunchDaemons/com.almenscorner.supportcompanion.plist` - LaunchDaemon for the app to run the scripts
- `~/Library/Application Support/SupportCompanion/` - Folder containing app data such as notification time stamps

## Uninstallation

An uninstaller script is included in the app bundle. The script can be found in the following location:
`/Applications/Utilities/Support Companion.app/Contents/Resources/Uninstall.sh`

## Using the app

When the app is started, a menu bar icon will appear. Clicking the icon will show available actions to take like
opening the app. No dock icon will be shown for the app and the app should be accessed from the menu bar icon.
This is to keep the app out of the way and not clutter the dock and make it easy for admins to start the app from a
terminal or script without showing the app to the end-user. Initializing the app this way sends notifications to the
user if they have available software updates for example.

## Troubleshooting
Logs can be viewed by running the following command in the terminal:
`log stream --debug --info --predicate 'subsystem contains "com.almenscorner.supportcompanion"'`

Or by searching for `subsystem: com.almenscorner.supportcompanion` in the Console app.

## Configuration

Many aspects of the app can be configured using MDM profiles, the folloing keys are available:
| Key | Type | Default | Required | Description |
| --- | --- | --- | --- | --- |
| `BrandName` | String | None | False | Configures the brand name shown in the menu |
| `BrandColor` | String | Blue | False | Configures the brand color shown in the app, available colors are: Blue, Green, Red, Orange |
| `SupportPageUrl` | String | None | False | Configures the URL to open when the user clicks on the Get Support button |
| `ChangePasswordUrl` | String | None | False | Configures the URL to open when the user clicks on the Change Password button |
| `ChangePasswordMode` | String | local | False | Configures the mode for the Change Password button, available modes are: `local`, `SSOExtension`, `url` |
| `SupportEmail` | String | None | False | Configures the email address shown when the user clicks on the Support Info button |
| `SupportPhone` | String | None | False | Configures the phone number shown when the user clicks on the Support Info button |
| `HiddenWidgets` | Array | None | False | Configures which widgets to hide, available widgets are: `DeviceInfo`, `MunkiPendingApps`, `MunkiUpdates`, `IntunePendingApps`, `IntuneUpdates`, `Storage`, `MdmStatus`, `Actions`, `Battery`, `EvergreenInfo` |
| `HiddenActions` | Array | None | False | Configures which actions to hide, available actions are: `Support`, `ManagedSoftwareCenter`, `ChangePassword`, `Reboot`, `KillAgent`, `SoftwareUpdates`, `GatherLogs` |
| `NotificationInterval` | Integer | 4 | False | Configures the interval for notifications in hours for Application Updates and Software Updates notifications |
| `NotificationTitle` | String | Support Companion | False | Configures the title for notifications for notifications |
| `NotificationImage` | String | None | False | Configures an image to add to notifications. Path should be specified |
| `SoftwareUpdateNotificationMessage` | String | You have software updates available. Take action now! \ud83c\udf89 | False | Configures the message for notifications for Software Updates notifications |
| `SoftwareUpdateNotificationButtonText` | String | Details \ud83d\udc40 | False | Configures the button text for notifications for Software Updates notifications |
| `AppUpdateNotificationMessage` | String | You have app updates available. Take action now! \ud83c\udf89 | False | Configures the message for notifications for App Updates notifications |
| `AppUpdateNotificationButtonText` | String | Details \ud83d\udc40 | False | Configures the button text for notifications for App Updates notifications |
| `CustomColors` | Array | None | False | Configures custom colors for the app, should be specified in hex format, see example below. Do not use `BrandColor` in conjunction with this key |
| `IntuneMode` | Bool | False | False | Configures the app to use Intune for application information. Only supports PKG and DMG type apps, not LOB. |
| `LogFolders` | Array | /Library/Logs/Microsoft | False | Configures the log folders to gather logs from. Only used when gathering logs. |
| `Actions` | Array | None | False | Configures custom actions to add to the tray menu. See example below. |

### Example Configuration

To switch from Munki to Intune for application information, add the following key to the profile:
```xml
<key>IntuneMode</key>
<true/>
```

```xml
<?xml version="1.0" encoding="UTF-8"?>
<!DOCTYPE plist PUBLIC "-//Apple//DTD PLIST 1.0//EN" "http://www.apple.com/DTDs/PropertyList-1.0.dtd">
<plist version="1.0">
  <dict>
    <key>PayloadContent</key>
    <array>
      <dict>
        <key>BrandName</key>
        <string>AwesomeCorp</string>
        <key>ChangePasswordMode</key>
        <string>SSOExtension</string>
        <key>CustomColors</key>
        <array>
          <dict>
            <key>PrimaryColor</key>
            <string>#00A0D0</string>
            <key>AccentColor</key>
            <string>#45637A</string>
          </dict>
        </array>
        <key>Actions</key>
        <array>
           <dict>
               <key>Name</key>
               <string>Restart clipboard 🥹</string>
               <key>Command</key>
               <string>killall pboard</string>
           </dict>
           <dict>
               <key>Name</key>
               <string>Restart Intune Agent ⚡️</string>
               <key>Command</key>
               <string>/usr/bin/osascript -e 'do shell script \"sudo killall IntuneMdmAgent\" with administrator privileges'</string>
           </dict>
        </array>
        <key>NotificationTitle</key>
        <string>AwesomeCorp IT</string>
        <key>PayloadDisplayName</key>
        <string>SupportCompanion</string>
        <key>PayloadIdentifier</key>
        <string>SupportCompanion</string>
        <key>PayloadType</key>
        <string>SupportCompanion</string>
        <key>PayloadUUID</key>
        <string>a7a0d79f-1cf0-42f2-bc7e-e67d7413a3c5</string>
        <key>PayloadVersion</key>
        <integer>1</integer>
        <key>SupportEmail</key>
        <string>demo@example.com</string>
        <key>SupportPhone</key>
        <string>123-456-789</string>
        <key>SupportUrl</key>
        <string>https://awesomecorp.support</string>
      </dict>
    </array>
    <key>PayloadDisplayName</key>
    <string>SupportCompanion</string>
    <key>PayloadIdentifier</key>
    <string>9c4a8e5e-4c70-4b82-83f7-44a053c146f4</string>
    <key>PayloadType</key>
    <string>Configuration</string>
    <key>PayloadUUID</key>
    <string>3D47F3E6-62ED-4668-A30F-6DA1DAE87B18</string>
    <key>PayloadVersion</key>
    <integer>1</integer>
  </dict>
</plist>

```

## Overview

### Home
![SCHome](https://github.com/almenscorner/SupportCompanion/assets/78877636/d908bb6a-d7ed-42a6-a5d1-5f500ea24a9c)
### Apps
![SCApps](https://github.com/almenscorner/SupportCompanion/assets/78877636/b63afaf6-e76a-4412-9a54-1af252e4f9b1)
### Identity
![SCIdentity](https://github.com/almenscorner/SupportCompanion/assets/78877636/c88849fb-3092-47f4-99a2-69c6cd8f9923)
### Support Info
![SCSupportInfo](https://github.com/almenscorner/SupportCompanion/assets/78877636/58ea4438-3de7-46d7-9f67-9de8c6e01a46)
### Gather logs
![SCLogs102](https://github.com/almenscorner/SupportCompanion/assets/78877636/8cdc3405-8268-4ac8-9210-fd0d5b8c1b85)
### Notifications
![SCNotification](https://github.com/almenscorner/SupportCompanion/assets/78877636/414a7d55-2925-4312-bd9c-9f11ac450e23)

## Credits
- [AvoloniaUI](https://avaloniaui.net/)
- [SukiUI](https://github.com/kikipoulet/SukiUI)
