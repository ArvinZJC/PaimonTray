![banner.png](./img_README/banner.png)

# PaimonTray

[![Codacy Badge](https://app.codacy.com/project/badge/Grade/b83aab262d444585b7df8f0c8a55ed3a)](https://www.codacy.com/gh/ArvinZJC/PaimonTray/dashboard?utm_source=github.com&utm_medium=referral&utm_content=ArvinZJC/PaimonTray&utm_campaign=Badge_Grade)
![GitHub commit activity](https://img.shields.io/github/commit-activity/m/ArvinZJC/PaimonTray)
[![GitHub](https://img.shields.io/github/license/ArvinZJC/PaimonTray)](./LICENCE)

**English (United Kingdom)** | [中文（简体，中国）](./README_zh-Hans-CN.md)

> Now, Paimon is on your Windows "system tray"!

PaimonTray is a Windows desktop app that can present Genshin Impact's real-time notes for users. Thanks to real-time notes, it becomes more convenient for the game players to be able to track a list of things as follows.

<details>
  <summary>Click to expand/collapse</summary>

- Daily commissions: completed with bonus rewards claimed?
- Enemies of Note: any remaining Original Resin cost-halving opportunity this week?
- Expeditions: show me the game characters dispatched!
- Original Resin: fully replenished?
- Parametric Transformer: can be used?
- Realm Currency: limit reached?

</details>

Paimon now makes real-time notes available on your Windows taskbar corner (aka the notification area or "system tray"). It can be absolutely a pain in the arse without this tool app. What do you reckon? 🤪

PaimonTray also has some other highlights that may interest you. These include but not limited to:

<details>
  <summary>Click to expand/collapse</summary>

- Web page login method: log into your account on the specific web page just like in a browser<sup id="source1">[1](#footnote1)</sup> to add/update an account.
- Alternative login method: logging into your account by entering cookies manually is another way to add/update an account.
- Not only the CN server (aka a miHoYo account) but also the global (aka a HoYoLAB account).
- Multiple accounts<sup id="source2">[2](#footnote2)</sup> with nifty management: all your added accounts can be updated, checked and refreshed, or removed.
- Only your selected characters: all characters (aka Genshin Impact accounts) linked with a miHoYo/HoYoLAB account are there for you to select whether to allow retrieving real-time notes.
- Configurable real-time notes refresh interval.
- _Real-time notes reminders (will be available in the future)._
- Date and time rather than duration: e.g., Paimon converts the duration required to complete/fully replenish/... to an estimated date and time to make it clear.
- Hints for "not yet unlocked" if Paimon can tell you.
- Dark mode support.
- Acrylic/Mica support<sup id="source3">[3](#footnote3)</sup>.
- I18n (Internationalisation). Supported languages as follows:
  - English (United Kingdom)
  - English (United States) - default if no matches
  - 中文（简体，中国）

</details>

## 💡 Why not give it a try?

You can get PaimonTray from:

| Channel                                                                       | Latest version                                                                                                                                               | Stable releases | Pre-releases | Auto-update |
| :---------------------------------------------------------------------------- | :----------------------------------------------------------------------------------------------------------------------------------------------------------- | :-------------: | :----------: | :---------: |
| [**Microsoft Store**](https://www.microsoft.com/store/productId/9PP6PJDDRNRZ) | [![GitHub release](https://img.shields.io/github/v/release/ArvinZJC/PaimonTray)](../../releases)                                                             |       ✅        |              |     ✅      |
| [**Releases**](../../releases)                                                | [![GitHub release (latest by date including pre-releases)](https://img.shields.io/github/v/release/ArvinZJC/PaimonTray?include_prereleases)](../../releases) |       ✅        |      ✅      |             |

<details>
  <summary>Q & A</summary>

- **Which channel should I select to get the app?**

  Please always get the app from Microsoft Store if possible. It may provide a stabler support to handle prerequisites (e.g. the app's framework dependency).

  Downloading the `PaimonTray_<version>.msixbundle` file from the [releases](../../releases) is an alternative option. You may select this channel if:

  - You cannot/hate to use Microsoft Store.
  - You do not care auto-updating.
  - You desire to try pre-releases.
  - You can handle prerequisites yourself when necessary.

  You may have the app from both channels installed at the same time depending on your Windows version and some system settings. However, it is not recommended, even though I would not to point out any significant downside.

- **I cannot get the latest version as stated above from Microsoft Store.**

  It depends on [the Microsoft's app certification process](https://docs.microsoft.com/en-gb/windows/uwp/publish/the-app-certification-process), which may result in delays.

- **How to use the downloaded `.msixbundle` file to install the app?**

  You can double-click the file to install the app via [the App Installer](https://apps.microsoft.com/store/detail/app-installer/9NBLGGH4NNS1?hl=en-gb&gl=GB). If it fails for any reason, you can try the following command at a PowerShell prompt. You may need an elevated PowerShell prompt if any error like "access is denied" occurs.

  ```PowerShell
  # NOTE: If you are using PowerShell 7+, please run the following command before using Add-AppxPackage.
  # Import-Module Appx -UseWindowsPowerShell

  Add-AppxPackage PaimonTray_<version>.msixbundle
  ```

- **Why does the provided `.msixbundle` file a little large?**

  Framework-dependent deployment has already reduced the file size significantly. The file bundles the multiple architecture versions of the installer into one entity.

</details>

## ❗ ATTENTION

> May I have your attention pls? 🔥

1. By 16 November 2022, everything looks good with Visual Studio 2022 (Version: 17.4.1) + .NET 7.0. PaimonTray is built with Windows UI Library (WinUI) 3, which ships with the Windows App SDK. You may find [this link](https://docs.microsoft.com/en-gb/windows/apps/windows-app-sdk/set-up-your-development-environment) useful to load the project. Additionally, I would like to thankfully acknowledge the following authors/projects.

   - Inspired by [PaimonMenuBar](https://github.com/spencerwooo/PaimonMenuBar).
     > We even use the same app icon. However, you are not expected to consider PaimonTray as "PaimonMenuBar for Windows", or vice versa. We have various design ideas and patterns for the target platforms, and independent development road maps.
   - API uses credited to [genshin.py](https://github.com/thesadru/genshin.py) and [DGP Studio](https://github.com/DGP-Studio).
   - App icon credited to [Chawong](https://www.pixiv.net/en/artworks/92415888).
   - README banner background credited to [void_0](https://www.pixiv.net/en/artworks/85543107).

2. Due to [the use of the Windows App SDK](https://docs.microsoft.com/en-gb/windows/apps/windows-app-sdk/system-requirements#windows-app-sdk), PaimonTray is expected to work well on Windows 10, version 1809 (build 17763) and later (arm64, x64, and x86). **It is awfully safe to permit the app behaviour for any system prompt regarding safety confirmation. The app is signed, is not malware, and will never ever collect and upload any user privacy.** Should you report a problem encountered, you may find [issues](https://github.com/ArvinZJC/PaimonTray/issues) useful.

   > **Warning**
   >
   > Due to [the limitation of the Windows App SDK](https://learn.microsoft.com/en-gb/windows/apps/windows-app-sdk/stable-channel#elevation), when PaimonTray attempts to run with elevated privilege (e.g., running as an administrator to try fixing the deployment failure to ensure a good deployment status of the Windows App SDK runtime), the following OS servicing update is required:
   >
   > - Windows 11 - [10 May, 2022—KB5013943 (OS Build 22000.675)](https://support.microsoft.com/en-gb/topic/may-10-2022-kb5013943-os-build-22000-675-14aa767a-aa87-414e-8491-b6e845541755)
   > - Windows 10 - [10 May, 2022—KB5013942 (OS Builds 19042.1706, 19043.1706, and 19044.1706)](https://support.microsoft.com/en-gb/topic/may-10-2022-kb5013942-os-builds-19042-1706-19043-1706-and-19044-1706-60b51119-85be-4a34-9e21-8954f6749504)

3. PaimonTray is designed to be a lightweight tool app focusing on **Genshin Impact's real-time notes only**. Performance is the very 1st priority. Continuous optimisation will be provided ~~(hopefully)~~. Currently, there is no plan to make it a fully-fledged app. Anyway, you are welcome to shout out your ideas.

4. The NuGet packages of the project are listed in the following table.

   | Name                                |    Version     |
   | :---------------------------------- | :------------: |
   | H.NotifyIcon.WinUI                  |     2.0.74     |
   | Microsoft.Toolkit.Uwp.Notifications |     7.1.3      |
   | Microsoft.Windows.SDK.BuildTools    | 10.0.22621.755 |
   | Microsoft.WindowsAppSDK             |  1.2.221109.1  |
   | Serilog.Sinks.Async                 |     1.5.0      |
   | Serilog.Sinks.File                  |     5.0.0      |

Good luck! 💖

---

<sub id="footnote1">[1.](#source1) This feature relies on Microsoft Edge WebView2 Runtime. PaimonTray will guide you properly if the runtime cannot be detected. You can also download the runtime by clicking [here](https://go.microsoft.com/fwlink/p/?LinkId=2124703).</sub>

<sub id="footnote2">[2.](#source2) Currently up to 5 accounts due to performance consideration. It may be tweaked, as PaimonTray sharpens itself.</sub>

<sub id="footnote3">[3.](#source3) This feature depends on your Windows version and some system settings. The actual visual effects can vary.</sub>
