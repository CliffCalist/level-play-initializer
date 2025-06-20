# LevelPlay Initializer
LevelPlay Initializer is a lightweight Unity utility that handles initialization of LevelPlay (IronSource) ads with built-in support for UMP consent (GDPR/CCPA) and IDFA (iOS 14+).

## Features
- Automatic User Messaging Platform (UMP) consent flow
- IDFA request on iOS via Unity iOS Advertising Support
- Applies privacy metadata for GDPR, CCPA, COPPA, and family policies
- One-call asynchronous initialization
- Simple configuration via ScriptableObject

## Installation
1. Create or edit the `AdsSettings` ScriptableObject and place it in a `Resources` folder.
2. Call the initializer at app start:

```csharp
await AdInitFlow.InitializeAsync();
```

3. Proceed with normal LevelPlay ad usage (`RewardedAd.Show()`, etc.)

## Notes
- This utility focuses only on initialization and privacy handling.
- Does not include any ad display or reward logic.