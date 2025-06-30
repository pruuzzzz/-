namespace MauiApp4
{
    public partial class CustomMoleculeComponent : ContentView
    {
        const int MoleculeCount = 10;
        const double MoleculeSize = 20;
        const double BoxWidth = 300;
        const double BoxHeight = 500;

        class Molecule
        {
            public BoxView View { get; set; }
            public double VX { get; set; }
            public double VY { get; set; }
        }

        readonly List<Molecule> molecules = new();
        readonly System.Timers.Timer timer;
        Random rand = new Random();

        private int temperature;

        public CustomMoleculeComponent()
        {
            InitializeComponent();

            // MessagingCenter에서 온도 값 변경을 수신
            MessagingCenter.Subscribe<MainPage, int>(this, "UpdateTemperature", (sender, t) =>
            {
                // 수신된 온도 값 저장
                temperature = t;  // 소문자 t 사용
                UpdateSpeedFromTemperature();  // 온도 값이 변경되었을 때만 속도 갱신
            });

            // 공을 초기화합니다
            for (int i = 0; i < MoleculeCount; i++)
            {
                var dot = new BoxView
                {
                    Color = Colors.SkyBlue,
                    WidthRequest = MoleculeSize,
                    HeightRequest = MoleculeSize,
                    CornerRadius = MoleculeSize / 2
                };

                double x = rand.NextDouble() * (BoxWidth - MoleculeSize);
                double y = rand.NextDouble() * (BoxHeight - MoleculeSize);
                AbsoluteLayout.SetLayoutBounds(dot, new Rect(x, y, MoleculeSize, MoleculeSize));
                MoleculeContainer.Children.Add(dot);

                molecules.Add(new Molecule
                {
                    View = dot,
                    VX = (rand.NextDouble() - 0.5) * 4, // 초기 속도 설정
                    VY = (rand.NextDouble() - 0.5) * 4  // 초기 속도 설정
                });
            }

            // Timer 생성, 16ms마다 UpdateMolecules 호출
            timer = new System.Timers.Timer(16) { AutoReset = true };
            timer.Elapsed += (s, e) => Dispatcher.Dispatch(UpdateMolecules);
            timer.Start();
        }

        void UpdateSpeedFromTemperature()
        {
            // 온도 값에 따라 실시간으로 속도 갱신
            foreach (var m in molecules)
            {
                m.VX = temperature / 170.0;  // VX는 t 값의 100분의 1
                m.VY = temperature / 100.0;  // VY는 t 값의 200분의 1
            }
        }

        // 외부에서 온 속도 값을 수신하여 공의 속도 갱신
        void UpdateSpeedFromExternal(double VX, double VY)
        {
            foreach (var m in molecules)
            {
                m.VX = VX;
                m.VY = VY;
            }
        }

        void UpdateMolecules()
        {
            foreach (var m in molecules)
            {
                // 공의 위치 업데이트
                var b = AbsoluteLayout.GetLayoutBounds(m.View);
                double x = b.X + m.VX;
                double y = b.Y + m.VY;

                // 세로 벽에 부딪히면 VX의 부호 변경
                if (x <= 0) // 왼쪽 벽에 부딪혔을 때
                {
                    m.VX = -m.VX;  // VX의 부호 반전
                    x = 0;  // 위치를 벽에 맞게 설정
                }
                else if (x + MoleculeSize >= BoxWidth) // 오른쪽 벽에 부딪혔을 때
                {
                    m.VX = -m.VX;  // VX의 부호 반전
                    x = BoxWidth - MoleculeSize;  // 위치를 벽에 맞게 설정
                }

                // 가로 벽에 부딪히면 VY의 부호 변경
                if (y <= 0) // 위쪽 벽에 부딪혔을 때
                {
                    m.VY = -m.VY;  // VY의 부호 반전
                    y = 0;  // 위치를 벽에 맞게 설정
                }
                else if (y + MoleculeSize >= BoxHeight) // 아래쪽 벽에 부딪혔을 때
                {
                    m.VY = -m.VY;  // VY의 부호 반전
                    y = BoxHeight - MoleculeSize;  // 위치를 벽에 맞게 설정
                }

                // 위치 업데이트 (위치는 그대로 두고, 속도만 반전)
                AbsoluteLayout.SetLayoutBounds(m.View, new Rect(x, y, MoleculeSize, MoleculeSize));
            }
        }
    }
}
