# LevelPlay Initializer
LevelPlay Initializer is a lightweight Unity utility that handles initialization of LevelPlay (IronSource) ads with built-in support for a custom privacy consent system and IDFA (iOS 14+).

## Features
- Custom privacy consent system
- IDFA request on iOS via Unity iOS Advertising Support
- Applies privacy metadata for GDPR, CCPA, COPPA, and family policies
- One-call asynchronous initialization
- Simple configuration via ScriptableObject

## Usage

### Initialization
1. Implement a class that inherits from `PrivacyConsentConfirmer` and override its `Confirm(...)` method. This can be a UI popup in your game that lets the user accept or reject consent. Inside this method, you can also open the `privacyPolicyUrl` if needed to show your privacy policy.
2. Create or edit the `AdsSettings` ScriptableObject and place it in a `Resources` folder. Assign your custom `PrivacyConsentConfirmer` prefab to the corresponding field in the settings.
3. Call the initializer at app start:
```csharp
await LevelPlayInitializer.InitializeAsync();
```
4. Proceed with normal LevelPlay ad usage (`RewardedAd.Show()`, etc.)

### Updating consent
Use the following method from your game settings to let users change their privacy choices at runtime:

```csharp
LevelPlayInitializer.RequestPrivacyConsent();
```

Any new consent changes will take effect on the next session when `LevelPlayInitializer.InitializeAsync` is called again.

## Notes
- This utility focuses only on initialization and privacy handling.
- Does not include any ad display or reward logic.