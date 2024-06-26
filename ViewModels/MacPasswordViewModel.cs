using System.Text.Json;
using Avalonia.Threading;
using ReactiveUI;
using SupportCompanion.Interfaces;
using SupportCompanion.Models;
using SupportCompanion.Services;

namespace SupportCompanion.ViewModels;

public class MacPasswordViewModel : ViewModelBase, IWindowStateAware
{
    private readonly ActionsService _actionsService;
    private readonly LoggerService _logger;
    private readonly MacPasswordService _macPasswordService;

    private KerberosSSOModel? _kerberosSSO;
    private PlatformSSOModel? _platformSSO;

    public MacPasswordViewModel(ActionsService actionsService, MacPasswordService macPasswordService,
        LoggerService loggerService)
    {
        _actionsService = actionsService;
        _macPasswordService = macPasswordService;
        _logger = loggerService;
        KerberosSSO = new KerberosSSOModel();
        PlatformSSO = new PlatformSSOModel();
        Dispatcher.UIThread.Post(InitializeAsync);
    }

    public KerberosSSOModel? KerberosSSO
    {
        get => _kerberosSSO;
        private set => this.RaiseAndSetIfChanged(ref _kerberosSSO, value);
    }

    public PlatformSSOModel? PlatformSSO
    {
        get => _platformSSO;
        private set => this.RaiseAndSetIfChanged(ref _platformSSO, value);
    }

    public void OnWindowHidden()
    {
        CleanUp();
    }

    public void OnWindowShown()
    {
        KerberosSSO = new KerberosSSOModel();
        PlatformSSO = new PlatformSSOModel();
        Dispatcher.UIThread.Post(InitializeAsync);
    }

    private async void InitializeAsync()
    {
        await GetMacPasswordInfo().ConfigureAwait(false);
    }

    private T GetValueOrDefault<T>(Dictionary<string, object> dictionary, string key, T defaultValue = default)
    {
        if (dictionary == null)
        {
            _logger.Log("MacPasswordViewModel", "Dictionary is null.", 1);
            return defaultValue;
        }

        if (!dictionary.TryGetValue(key, out var value))
        {
            _logger.Log("MacPasswordViewModel", $"Key '{key}' not found in dictionary.", 1);
            return defaultValue;
        }

        if (value == null)
        {
            _logger.Log("MacPasswordViewModel", $"Value for key '{key}' is null.", 1);
            return defaultValue;
        }

        if (value is JsonElement element)
            try
            {
                if (typeof(T) == typeof(string)) return (T)(object)element.GetString();
                if (typeof(T) == typeof(int)) return (T)(object)element.GetInt32();
                if (typeof(T) == typeof(bool)) return (T)(object)element.GetBoolean();
                if (typeof(T) == typeof(decimal)) return (T)(object)element.GetDecimal();
            }
            catch (Exception ex)
            {
                _logger.Log("MacPasswordViewModel",
                    $"Error converting value for key '{key}' to type '{typeof(T)}': {ex.Message}", 1);
                return defaultValue;
            }

        if (value is T typedValue) return typedValue;

        _logger.Log("MacPasswordViewModel", $"Value for key '{key}' is not of type '{typeof(T)}'.", 1);
        return defaultValue;
    }

    private void UpdatePlatformSSOModel(Dictionary<string, object> deviceConfig, Dictionary<string, object> userConfig)
    {
        PlatformSSO.IsPlatformSSO = true;
        PlatformSSO.ExtensionIdentifier = GetValueOrDefault<string>(deviceConfig, "extensionIdentifier");
        PlatformSSO.RegistrationCompleted = GetValueOrDefault<bool>(deviceConfig, "registrationCompleted");
        PlatformSSO.LoginFrequency = GetValueOrDefault<int>(deviceConfig, "loginFrequency");
        PlatformSSO.NewUserAuthorizationMode = GetValueOrDefault<string>(deviceConfig, "newUserAuthorizationMode");
        PlatformSSO.SharedDeviceKeys = GetValueOrDefault<bool>(deviceConfig, "sharedDeviceKeys");
        PlatformSSO.UserAuthorizationMode = GetValueOrDefault<string>(deviceConfig, "userAuthorizationMode");
        PlatformSSO.SdkVersionString = GetValueOrDefault<decimal>(deviceConfig, "sdkVersionString");
        var loginType = GetValueOrDefault<string>(userConfig, "loginType");
        if (loginType != null)
            loginType.Split(" ").ToList().ForEach(x =>
            {
                if (x.Contains("(1)"))
                    PlatformSSO.LoginType = "Password";
                else if (x.Contains("(2)"))
                    PlatformSSO.LoginType = "Secure Enclave";
                else if (x.Contains("(3)")) PlatformSSO.LoginType = "Smart Card";
            });
        PlatformSSO.RegistrationStatusColor = PlatformSSO.RegistrationCompleted ? "LightGreen" : "#FF4F44";
    }

    private async Task GetMacPasswordInfo()
    {
        // Check if we're using Kerberos SSO
        var realm = await _actionsService.RunCommandWithOutput("/usr/bin/app-sso -l --json");
        var PlatformSSOInfo = await _macPasswordService.GetPlatformSsoInfo();
        var realmJson = JsonSerializer.Deserialize<string[]>(realm);
        var realmName = string.Empty;
        if (realmJson != null && realmJson.Length > 0) realmName = realmJson[0];

        if (!string.IsNullOrEmpty(realmName))
        {
            KerberosSSO.IsKerberosSSO = true;
            var kerberosInfo = await _macPasswordService.GetKerberosSsoInfo();

            KerberosSSO.UserName = kerberosInfo.TryGetValue(
                "user_name", out var userName)
                ? Convert.ToString(userName)
                : string.Empty;
            KerberosSSO.KerberosRealm = kerberosInfo.TryGetValue(
                "realm", out var kerberosRealm)
                ? Convert.ToString(kerberosRealm)
                : string.Empty;
            KerberosSSO.LocalPasswordLastChanged = kerberosInfo.TryGetValue(
                "local_password_changed_date",
                out var localPasswordChangedDate)
                ? Convert.ToInt32(localPasswordChangedDate)
                : 0;
            KerberosSSO.KerberosPasswordExpiryDays = kerberosInfo.TryGetValue(
                "password_expires_date",
                out var passwordExpiresDate)
                ? Convert.ToInt32(passwordExpiresDate)
                : 0;
            KerberosSSO.KerberosPasswordLastChangedDays = kerberosInfo.TryGetValue(
                "password_changed_date",
                out var passwordChangedDate)
                ? Convert.ToInt32(passwordChangedDate)
                : 0;
            KerberosSSO.ExpiryColor = kerberosInfo.TryGetValue(
                "password_expiry_color",
                out var passwordExpiryColor)
                ? Convert.ToString(passwordExpiryColor)
                : "White";
        }
        else
        {
            if (PlatformSSOInfo.TryGetValue("device_configuration", out var deviceConfigObj) &&
                deviceConfigObj is Dictionary<string, object> deviceConfig)
            {
                Dictionary<string, object> userConfig = null;
                if (PlatformSSOInfo.TryGetValue("user_configuration", out var userConfigObj) &&
                    userConfigObj is Dictionary<string, object> tempUserConfig) userConfig = tempUserConfig;
                UpdatePlatformSSOModel(deviceConfig, userConfig);
            }
        }
    }

    private void CleanUp()
    {
        KerberosSSO = null;
        PlatformSSO = null;
    }
}