# AdFlow

**AdFlow** is a lightweight Unity plugin that simplifies ad integration using IronSource LevelPlay.

It automates user consent via UMP (User Messaging Platform), requests IDFA on iOS, initializes LevelPlay, and optionally shows rewarded ads with a visual countdown timer.

## ✅ Features

- GDPR & CCPA support via Google UMP + LevelPlay privacy metadata
- Automatic IDFA request on iOS 14+
- LevelPlay initialization with consent awareness
- Rewarded ads with optional pre-ad countdown screen
- Configurable via `AdsSettings` ScriptableObject

## 🚀 Usage

Call once on app start:

```csharp
await AdFlow.InitializeAsync();
```

To show a rewarded ad with countdown:

```csharp
AdFlow.ShowRewardedAdWithTimer();
```

To use direct ad showing:

```csharp
IronSource.Agent.showRewardedVideo();
```

## 🧩 Integration

- Requires IronSource LevelPlay SDK and Google UMP SDK
- `AdsSettings` must be placed in a `Resources` folder
- Optional timer screen prefab can be assigned in settings

## 📁 Status

This is an early version. Core functionality is stable, but documentation and flexibility (e.g. callbacks, analytics events, editor integration) are still in progress.
