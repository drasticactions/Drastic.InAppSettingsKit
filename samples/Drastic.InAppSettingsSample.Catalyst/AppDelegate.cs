using InAppSettingsKit;
using ObjCRuntime;

namespace Drastic.InAppSettingsSample.Catalyst;

[Register ("AppDelegate")]
public class AppDelegate : UIApplicationDelegate {
	public override UIWindow? Window {
		get;
		set;
	}

	public override bool FinishedLaunching (UIApplication application, NSDictionary launchOptions)
	{
		// create a new window instance based on the screen size
		Window = new UIWindow (UIScreen.MainScreen.Bounds);

		// create a UIViewController with a single UILabel
		var vc = new ViewController ();
		Window.RootViewController = vc;

		// make the window visible
		Window.MakeKeyAndVisible ();

		return true;
	}
}


partial class ViewController : UITableViewController, ISettingsDelegate
{
    private AppSettingsViewController appSettingsViewController;

    public ViewController()
    {
        var button = new UIButton(this.View!.Bounds)
        {
            AutoresizingMask = UIViewAutoresizing.All,
        };

        button.SetTitle("Show Settings", UIControlState.Normal);
        this.View!.AddSubview(button);
        button.PrimaryActionTriggered += Button_PrimaryActionTriggered;

        this.View!.BackgroundColor = UIColor.Gray;
    }

    private void Button_PrimaryActionTriggered(object? sender, EventArgs e)
    {
        this.PresentViewController(this.AppSettingsViewController, true, null);
    }

    public AppSettingsViewController AppSettingsViewController
    {
        get
        {
            if (appSettingsViewController == null)
            {
                appSettingsViewController = new AppSettingsViewController();
                appSettingsViewController.Delegate = this;

                UpdateAutoConnect(NSUserDefaults.StandardUserDefaults.BoolForKey("AutoConnect"));

                // Uncomment to not display InAppSettingsKit credits for creators:
                // But we encourage you no to uncomment. Thank you!
                //appSettingsViewController.ShowCreditsFooter == false;
            }
            return appSettingsViewController;
        }
    }

    public override void ViewDidLoad()
    {
        base.ViewDidLoad();

        NSNotificationCenter.DefaultCenter.AddObserver((NSString)SettingsStore.AppSettingChangedNotification, notification => {
            if (((NSString)notification.Object) == "AutoConnect")
            {
                var enabledObject = (NSNumber)notification.UserInfo.ObjectForKey((NSString)"AutoConnect");
                UpdateAutoConnect(enabledObject.BoolValue);
            }
        });

        UpdateAutoConnect(NSUserDefaults.StandardUserDefaults.BoolForKey("AutoConnect"));
    }

    private void UpdateAutoConnect(bool enabled)
    {
        AppSettingsViewController.SetHiddenKeys(enabled ? null : new[] { "AutoConnectLogin", "AutoConnectPassword" }, true);
    }

    public override void RowSelected(UITableView tableView, NSIndexPath indexPath)
    {
        tableView.DeselectRow(indexPath, true);

        switch (indexPath.Row)
        {
            case 0:
                // show push
                AppSettingsViewController.ShowDoneButton = false;
                AppSettingsViewController.NavigationItem.RightBarButtonItem = null;
                NavigationController.PushViewController(AppSettingsViewController, true);
                break;

            case 1:
                // show modal
                var navController = new UINavigationController(AppSettingsViewController);
                AppSettingsViewController.ShowDoneButton = true;
                PresentViewController(navController, true, null);
                break;
        }
    }

    public override void ViewWillAppear(bool animated)
    {
        base.ViewWillAppear(animated);

        // your code here to reconfigure the app for changed settings
    }

    // Settings delegate

    public void SettingsViewControllerDidEnd(AppSettingsViewController sender)
    {
        DismissViewController(true, null);

        // your code here to reconfigure the app for changed settings (modal only)
    }

    [Export("settingsViewController:buttonTappedForSpecifier:")]
    public virtual void ButtonTappedForSpecifier(AppSettingsViewController sender, SettingsSpecifier specifier)
    {
        Console.WriteLine("Settings button tapped: " + specifier.Key);
    }

    [Export("settingsViewController:tableView:heightForHeaderForSection:")]
    public virtual nfloat GetHeightForHeaderForSection(ISettingsViewController settingsViewController, UITableView tableView, nint section)
    {
        var key = settingsViewController.GetSettingsReader().GetKey(section);
        if (key == "IASKLogo")
        {
            return UIImage.FromBundle("Icon.png").Size.Height + 25;
        }
        else if (key == "IASKCustomHeaderStyle")
        {
            return 55;
        }
        return 0;
    }

    [Export("settingsViewController:tableView:viewForHeaderForSection:")]
    public virtual UIView GetViewForHeaderForSection(ISettingsViewController settingsViewController, UITableView tableView, nint section)
    {
        var key = settingsViewController.GetSettingsReader().GetKey(section);
        if (key == "IASKLogo")
        {
            var imageView = new UIImageView(UIImage.FromBundle("Icon.png"));
            imageView.ContentMode = UIViewContentMode.Center;
            return imageView;
        }
        else if (key == "IASKCustomHeaderStyle")
        {
            var label = new UILabel();
            label.BackgroundColor = UIColor.Clear;
            label.TextAlignment = UITextAlignment.Center;
            label.TextColor = UIColor.Red;
            label.ShadowColor = UIColor.White;
            label.ShadowOffset = new CGSize(0, 1);
            label.Lines = 0;
            label.Font = UIFont.BoldSystemFontOfSize(16);

            //figure out the title from settingsbundle
            label.Text = settingsViewController.GetSettingsReader().GetTitle(section);

            return label;
        }
        return null;
    }

    [Export("tableView:heightForSpecifier:")]
    public virtual nfloat GetHeightForSpecifier(UITableView tableView, SettingsSpecifier specifier)
    {
        if (specifier.Key == "customCell")
        {
            return 44 * 3;
        }
        return 0;
    }

    [Export("tableView:cellForSpecifier:")]
    public virtual UITableViewCell GetCellForSpecifier(UITableView tableView, SettingsSpecifier specifier)
    {
        var cell = (CustomViewCell)tableView.DequeueReusableCell(specifier.Key);

        if (cell == null)
        {
            cell = NSBundle.MainBundle.LoadNib("CustomViewCell", this, null).GetItem<CustomViewCell>(0);
            //cell.E. += delegate {
            //    NSUserDefaults.StandardUserDefaults.SetString(cell.TextView.Text, "customCell");
            //    NSNotificationCenter.DefaultCenter.PostNotificationName(SettingsStore.AppSettingChangedNotification, (NSString)"customCell");
            //};
        }
        var text = NSUserDefaults.StandardUserDefaults.StringForKey(specifier.Key) ?? specifier.DefaultStringValue;
        //cell.TextLabel.Text = text;
        cell.SetNeedsLayout();

        return cell;
    }
}

public class CustomViewCell : UITableViewCell
{
    public CustomViewCell()
    {
    }

    public CustomViewCell(NSCoder coder) : base(coder)
    {
    }

    public CustomViewCell(CGRect frame) : base(frame)
    {
    }

    public CustomViewCell(UITableViewCellStyle style, string reuseIdentifier) : base(style, reuseIdentifier)
    {
    }

    public CustomViewCell(UITableViewCellStyle style, NSString? reuseIdentifier) : base(style, reuseIdentifier)
    {
    }

    protected CustomViewCell(NSObjectFlag t) : base(t)
    {
    }

    protected internal CustomViewCell(NativeHandle handle) : base(handle)
    {
    }
}