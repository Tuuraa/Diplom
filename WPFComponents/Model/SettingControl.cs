namespace WPFComponents.Model
{
    public class SettingControlItem
    {
        public string? Header { get; set; }
        public string? Description { get; set; }
        public bool? isEnabled { get; set; }

        public SettingControlItem(string header, string desc, bool is_enable)
        {
            Header = header; Description = desc; isEnabled = is_enable;
        }
    }
}
