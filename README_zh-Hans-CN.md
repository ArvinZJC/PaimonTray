![banner.png](./img_README/banner.png)

# PaimonTray

[![Codacy Badge](https://app.codacy.com/project/badge/Grade/b83aab262d444585b7df8f0c8a55ed3a)](https://www.codacy.com/gh/ArvinZJC/PaimonTray/dashboard?utm_source=github.com&utm_medium=referral&utm_content=ArvinZJC/PaimonTray&utm_campaign=Badge_Grade)
![GitHub commit activity](https://img.shields.io/github/commit-activity/m/ArvinZJC/PaimonTray)
[![GitHub](https://img.shields.io/github/license/ArvinZJC/PaimonTray)](./LICENCE)

[English (United Kingdom)](./README.md) | **中文（简体，中国）**

> 嗨，派蒙在您 Windows 的“系统托盘”上啦！

PaimonTray 是一个 Windows 桌面应用，它用来向用户展示原神的实时便笺。多亏了实时便笺，玩家们可更方便地跟进如下事项：

<details>
  <summary>点我展开/折叠</summary>

- 每日委托任务：已完成，并已领取额外奖励？
- 值得铭记的强敌：本周还有剩余消耗原粹树脂减半次数？
- 探索派遣：给我康康进行探索派遣的游戏角色！
- 原粹树脂：全部恢复？
- 参量质变仪：可使用？
- 洞天宝钱：达到上限？

</details>

派蒙现在将实时便笺带到您 Windows 任务栏角（亦称通知区域或“系统托盘”）上。没有这个工具应用会是多么痛苦的事情呀！您说呢？🤪

PaimonTray 还有一些其他可能会吸引您的亮点，包括但不限于：

<details>
  <summary>点我展开/折叠</summary>

- 网页登录方案：就像在浏览器中<sup id="source1">[1](#footnote1)</sup>，在指定网页上登录到您的账号来添加/更新账号。
- 替代的登录方案：手动输入 cookie 来登录到您的账号是另一种添加/更新账号的方式。
- 不仅国服（指米哈游账号），还有国际服（指 HoYoLAB 账号）。
- 多账号<sup id="source2">[2](#footnote2)</sup>，以及恰到好处的管理：所有您已添加的账号可更新、检查并刷新、移除。
- 仅关注您已选择的角色：米哈游/ HoYoLAB 账号所有已绑定的角色（指原神账号）都可供您选择是否允许查询实时便笺。
- 可配置的实时便笺刷新间隔时间。
- _实时便笺提醒（将在未来上线）。_
- 日期和时间来代替时长：例如，派蒙会将完成/全部恢复/……所需要的时长转换成预计的日期和时间。
- 派蒙会尽力对“未解锁”事项给予提示。
- 支持深色模式。
- 支持云母/亚克力<sup id="source3">[3](#footnote3)</sup>。
- 国际化。支持的语言如下：
  - English (United Kingdom)
  - English (United States) ——无匹配语言时默认
  - 中文（简体，中国）

</details>

## 💡 要不用用试试？

您可用如下方式获取 PaimonTray：

| 渠道                                                                       | 最新版本                                                                                                                                                     | 稳定版 | 预发布 | 自动更新 |
| :------------------------------------------------------------------------- | :----------------------------------------------------------------------------------------------------------------------------------------------------------- | :----: | :----: | :------: |
| [**微软应用商店**](https://www.microsoft.com/store/productId/9PP6PJDDRNRZ) | [![GitHub release](https://img.shields.io/github/v/release/ArvinZJC/PaimonTray)](../../releases)                                                             |   ✅   |        |    ✅    |
| [**发行**](../../releases)                                                 | [![GitHub release (latest by date including pre-releases)](https://img.shields.io/github/v/release/ArvinZJC/PaimonTray?include_prereleases)](../../releases) |   ✅   |   ✅   |          |

<details>
  <summary>问与答</summary>

- **我应该选择哪个渠道来获取此应用？**

  请尽可能通过微软应用商店来获取此应用。它能对处理前提要求（如：应用所需依赖）提供更稳定的支持。

  从[发行](../../releases)下载 `PaimonTray_<version>.msixbundle` 文件是一个替代的选择。若您属于下列情况，则您可能更适合从此渠道来获取此应用。

  - 您不能/讨厌使用微软应用商店。
  - 您不在乎是否能自动更新。
  - 您希望体验预发布。
  - 您能在必要时自行处理前提要求。

  由于这两个渠道使用不用的包名，您可同时安装来自两个渠道的此应用。

- **我无法从微软应用商店获取如上所示的最新版本。**

  这取决于[微软的应用认证过程](https://docs.microsoft.com/en-gb/windows/uwp/publish/the-app-certification-process)，可能造成上线延迟。

- **如何用下载的 `.msixbundle` 文件来安装此应用？**

  您可双击下载的文件来通过[应用安装程序](https://apps.microsoft.com/store/detail/%E5%BA%94%E7%94%A8%E5%AE%89%E8%A3%85%E7%A8%8B%E5%BA%8F/9NBLGGH4NNS1?hl=zh-cn&gl=CN)安装此应用。若有任何错误，则您可在 PowerShell 中尝试如下命令。若发生任何类似“访问被拒绝”的错误，则您可能需要以管理员身份运行 PowerShell。

  ```PowerShell
  # 注意: 若您使用 PowerShell 7+，则请在使用 Add-AppxPackage 前先运行如下命令：
  # Import-Module Appx -UseWindowsPowerShell

  Add-AppxPackage PaimonTray_<version>.msixbundle
  ```

- **为什么提供的 `.msixbundle` 文件有点儿大？**

  依赖于框架式的部署已经大幅减小文件大小。不过呢，首先，这个文件将安装程序的多个体系结构版本捆绑成一个实体。其次，[C#/WinRT 目前暂不支持 IL 剪裁](https://github.com/microsoft/CsWinRT/issues/373)。

</details>

## ❗ 注意

> 敲黑板了！敲黑板了！🔥

1. 截至 2022 年 11 月 11 日，使用 Visual Studio 2022（版本：17.4.0）+ .NET 6.0 开发表现良好。PaimonTray 使用随附 Windows 应用 SDK 的 Windows UI 库（WinUI）3 构建。您可能会觉得[此链接](https://docs.microsoft.com/zh-cn/windows/apps/windows-app-sdk/set-up-your-development-environment)对载入项目有用。此外，我要特别感谢以下作者/项目：

   - 受启发于 [PaimonMenuBar](https://github.com/spencerwooo/PaimonMenuBar)。

     > 我们甚至使用一样的应用图标。但是，您不要将 PaimonTray 当作 “PaimonMenuBar 的 Windows 版”，反之亦然。我们针对目标平台有不同的设计意图和模式，以及互相独立的开发计划。

   - 接口用法参考于 [genshin.py](https://github.com/thesadru/genshin.py) 和 [DGP Studio](https://github.com/DGP-Studio)。

   - 应用图标来源于 [Chawong](https://www.pixiv.net/en/artworks/92415888)。

   - README 横幅背景来源于 [void_0](https://www.pixiv.net/en/artworks/85543107)。

2. 受 [Windows 应用 SDK 的影响](https://docs.microsoft.com/zh-cn/windows/apps/windows-app-sdk/system-requirements#windows-app-sdk) ，PaimonTray 应能支持 Windows 10 版本 1809（内部版本 17763）及更高版本（arm64、x64 和 x86）。**在安装、使用和卸载此应用的过程中，任何来自系统的安全提示都可授权允许。此应用已签名，无恶意行为，亦不会收集并上传任何用户隐私。** 若遇问题，可移步[议题](https://github.com/ArvinZJC/PaimonTray/issues)。

3. PaimonTray 要作为一个轻量的工具应用，**仅关注原神的实时便笺**。性能是绝对的第一要务。优化将持续不断 ~~（但愿吧，至少先画个饼）~~。当前没有成为一个“全家桶”应用的计划。不管怎样，欢迎您说出您的想法。

4. 项目 NuGet 包参见下面的表格。

   | 名称                                |      版本      |
   | :---------------------------------- | :------------: |
   | H.NotifyIcon.WinUI                  |     2.0.74     |
   | Microsoft.Toolkit.Uwp.Notifications |     7.1.3      |
   | Microsoft.Windows.SDK.BuildTools    | 10.0.22621.755 |
   | Microsoft.WindowsAppSDK             |  1.2.221109.1  |
   | Serilog.Sinks.Async                 |     1.5.0      |
   | Serilog.Sinks.File                  |     5.0.0      |

好运哦! 💖

---

<sub id="footnote1">[1.](#source1) 此功能依赖 Microsoft Edge WebView2 运行环境。PaimonTray 会在未能检测到运行环境的情况下恰当地引导您。您也可点击[这里](https://go.microsoft.com/fwlink/p/?LinkId=2124703)来下载运行环境。</sub>

<sub id="footnote2">[2.](#source2) 出于对性能的考虑，当前最多 5 个账号。随着 PaimonTray 越来越棒，这可能会被调整。</sub>

<sub id="footnote3">[3.](#source3) 此功能取决于您的 Windows 版本和一些系统设置。实际视觉效果会根据情况变化。</sub>
