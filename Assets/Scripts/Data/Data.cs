namespace DataLayer
{
    public static class Data
    {
        public static ObservableProperty<float> Distance { get; set; }
        public static ObservableProperty<bool> IsWheelie { get; set; }

        static Data()
        {
            Distance = new ObservableProperty<float>(0);
            IsWheelie = new ObservableProperty<bool>(false);
        }

        public static void Reset()
        {
            Distance.Set(0);
            IsWheelie.Set(false);
        }
    }
}
