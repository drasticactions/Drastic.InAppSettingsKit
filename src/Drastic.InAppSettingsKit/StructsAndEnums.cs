using ObjCRuntime;

namespace InAppSettingsKit
{
	[Native]
	public enum IASKValidationResult : ulong
	{
		Ok,
		Failed,
		FailedWithShake
	}

	[Native]
	public enum IASKToggleStyle : ulong
	{
		Switch,
		Checkmark
	}
}
