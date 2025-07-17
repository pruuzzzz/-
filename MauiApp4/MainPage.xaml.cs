namespace MauiApp4
{
    public partial class MainPage : ContentPage
    {
        public static class SharedData
        {
            public static int T = 300; // 온도 초기값
            public static int V = 30;  // 부피 초기값
            public static int P = 1000; // 압력 초기값
            public static int Q = 0;  // 열량 초기값
            public static double A = SharedData.T;
            public static double B = SharedData.V;
        }

        private double adiabaticInitialT;
        private double adiabaticInitialV;
        private double adiabaticInitialP;
        private bool adiabaticInitialized = false;

        public MainPage()
        {
            InitializeComponent();
        }

        private void OnTemperatureChanged(object sender, ValueChangedEventArgs e)
        {
            int T = (int)e.NewValue;
            TemperatureLabel.Text = $"현재 온도: {T} K";
            SharedData.T = T;
            UpdateThermodynamicDisplay();

            // SharedData.t가 변경될 때마다 MessagingCenter로 데이터를 전송
            MessagingCenter.Send(this, "UpdateTemperature", SharedData.T); // MainPage에서 CustomMoleculeComponent로 전송
        }

        private void OnValueChanged(object sender, ValueChangedEventArgs e)
        {
            int V = (int)e.NewValue;
            ValueLabel.Text = $"현재 부피: {V} m³";
            SharedData.V = V;
            UpdateThermodynamicDisplay();
        }

        private void OnPressureChanged(object sender, ValueChangedEventArgs e)
        {
            int P = (int)e.NewValue;
            PressureLabel.Text = $"현재 압력: {P} hpa";
            SharedData.P = P;
            UpdateThermodynamicDisplay();
        }

        private void OnEntryTextChanged(object sender, TextChangedEventArgs e)
        {
            if (int.TryParse(e.NewTextValue, out int Q))
            {
                MyResultLabel.Text = $"입력한 숫자: {Q}";
                SharedData.Q = Q;
                UpdateThermodynamicDisplay();
            }
            else
            {
                MyResultLabel.Text = "정수를 입력하세요!";
            }
        }

        private void OnCheckBoxChanged(object sender, CheckedChangedEventArgs e)
        {
            if (sender is CheckBox selected && selected.IsChecked)
            {
                var checkBoxes = new[] { Chk1, Chk2, Chk3, Chk4 };
                foreach (var cb in checkBoxes)
                {
                    if (cb != selected)
                        cb.IsChecked = false;
                }
            }
            if (Chk4.IsChecked) // 단열 과정 체크 시 초기값 저장
            {
                adiabaticInitialT = SharedData.T;
                adiabaticInitialV = SharedData.V;
                adiabaticInitialP = SharedData.P;
            }

            UpdateThermodynamicDisplay();
        }

        private void UpdateThermodynamicDisplay()
        {
            double temperature = SharedData.T;
            double volume = SharedData.V;
            double pressure = SharedData.P;


            if (Chk1.IsChecked) // 등적 과정
            {
                double P = SharedData.P * 100;
                double V = SharedData.V;
                double T = SharedData.T;
                double Q = SharedData.Q;
                double N = P * V / T;

                double x = ((N * T + 2 * Q / 3) - P * V) / (5 * P / 3);
                double V_prime = V + x;
                double T_prime = T + 2 * Q / (3 * N) - 2 * P * x / (3 * N);

                SharedData.A = T_prime;
                SharedData.B = V_prime;

                Dispatcher.Dispatch(() =>
                {
                    VolumeLabelGrid.Text = $"{Math.Round(V_prime)} m³";
                    PressureLabelGrid.Text = $"{SharedData.P} hPa";
                    TemperatureLabelGrid.Text = $"{Math.Round(T_prime)} K";
                });
            }
            else if (Chk2.IsChecked) // 등압 과정
            {
                double P = SharedData.P * 100;
                double V = SharedData.V;
                double T = SharedData.T;
                double Q = SharedData.Q;
                double N = P * V / T;

                double T_prime = T + 2 * Q / (3 * N);
                double P_prime = N * T_prime / V;

                SharedData.A = T_prime;

                Dispatcher.Dispatch(() =>
                {
                    VolumeLabelGrid.Text = $"{Math.Round(V)} m³";
                    PressureLabelGrid.Text = $"{Math.Round(P_prime / 100)} hPa";
                    TemperatureLabelGrid.Text = $"{Math.Round(T_prime)} K";
                });
            }
            else if (Chk3.IsChecked) // 등온 과정
            {
                double P = SharedData.P * 100;
                double V = SharedData.V;
                double T = SharedData.T;
                double Q = SharedData.Q;
                double N = P * V / T;

                double x = ((N * T + 2 * Q / 3) - P * V) / (5 * P / 3);
                double V_prime = V + x;
                double y = P - N * T / (V + x);
                double P_prime = P - y;

                SharedData.A = T;  // 등온: 온도 그대로 유지
                SharedData.B = V_prime;

                Dispatcher.Dispatch(() =>
                {
                    VolumeLabelGrid.Text = $"{Math.Round(V_prime)} m³";
                    PressureLabelGrid.Text = $"{Math.Round(P_prime / 100)} hPa";
                    TemperatureLabelGrid.Text = $"{Math.Round(T)} K";
                });
            }
            else if (Chk4.IsChecked) // 단열 과정
            {
                double V = SharedData.V;
                double T = SharedData.T;
                double P = SharedData.P * 100;
                double gamma = 5.0 / 3.0;

                double V0 = adiabaticInitialV;
                double T0 = adiabaticInitialT;
                double P0 = adiabaticInitialP * 100;

                double V_ratio = V / V0;

                double T_prime = T0 * Math.Pow(V_ratio, 1 - gamma);
                double P_prime = P0 * Math.Pow(V0 / V, gamma);

                SharedData.A = T_prime;
                SharedData.B = V;

                Dispatcher.Dispatch(() =>
                {
                    VolumeLabelGrid.Text = $"{Math.Round(V)} m³";
                    PressureLabelGrid.Text = $"{Math.Round(P_prime / 100)} hPa";
                    TemperatureLabelGrid.Text = $"{Math.Round(T_prime)} K";
                });
            }

            // 계산된 A 값(최종 온도)을 CustomMoleculeComponent에 전달
            MessagingCenter.Send(this, "UpdateTemperature", (int)Math.Round(SharedData.A));
            MessagingCenter.Send(this, "UpdateVolume", (int)Math.Round(SharedData.B));
        }

    }
}

