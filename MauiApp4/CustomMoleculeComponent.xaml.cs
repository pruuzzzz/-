namespace MauiApp4
{
    public partial class CustomMoleculeComponent : ContentView
    {
        const int MoleculeCount = 10;
        const double MoleculeSize = 20;
        const double BoxWidth = 300;
        double BoxHeight = 500;

        class Molecule
        {
            public BoxView View { get; set; }
            public double VX { get; set; }
            public double VY { get; set; }
            public double Angle { get; set; }  // 개별 방향
        }

        readonly List<Molecule> molecules = new();
        readonly System.Timers.Timer timer;
        Random rand = new Random();

        private int temperature = 100; // 기본 온도 (처음에도 움직이게 하려면 필요)
        private int volume = 500;

        public CustomMoleculeComponent()
        {
            InitializeComponent();

            // 온도 변경 메시지 수신
            MessagingCenter.Subscribe<MainPage, int>(this, "UpdateTemperature", (sender, t) =>
            {
                temperature = t;
                UpdateSpeedFromTemperature();
            });
            MessagingCenter.Subscribe<MainPage, int>(this, "UpdateVolume", (sender, v) =>
            {
                volume = v;
                UpdateBoxSizeFromVolume();
            });

            // 공 초기화
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

                double angle = rand.NextDouble() * 2 * Math.PI;
                double speed = temperature / 100.0;

                molecules.Add(new Molecule
                {
                    View = dot,
                    Angle = angle,
                    VX = speed * Math.Cos(angle),
                    VY = speed * Math.Sin(angle)
                });
            }

            // 타이머 시작
            timer = new System.Timers.Timer(16) { AutoReset = true };
            timer.Elapsed += (s, e) => Dispatcher.Dispatch(UpdateMolecules);
            timer.Start();
        }

        void UpdateSpeedFromTemperature()
        {
            double speed = temperature / 100.0;
            foreach (var m in molecules)
            {
                m.VX = speed * Math.Cos(m.Angle);
                m.VY = speed * Math.Sin(m.Angle);
            }
        }
        void UpdateBoxSizeFromVolume()
        {
            // 기준 부피 30에서 비례 확대/축소 (최소 크기 200x350 보장)
            double scale = volume*10;

            BoxHeight = scale;
            BoxFrame.HeightRequest = BoxHeight;

            // 기존 공들의 위치가 박스 안에 있도록 한 번 조정
            foreach (var m in molecules)
            {
                var b = AbsoluteLayout.GetLayoutBounds(m.View);
                double x = Math.Min(b.X, BoxWidth - MoleculeSize);
                double y = Math.Min(b.Y, BoxHeight - MoleculeSize);
                AbsoluteLayout.SetLayoutBounds(m.View, new Rect(x, y, MoleculeSize, MoleculeSize));
            }
        }

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
                var b = AbsoluteLayout.GetLayoutBounds(m.View);
                double x = b.X + m.VX;
                double y = b.Y + m.VY;

                // 좌우 벽 충돌 처리
                if (x <= 0)
                {
                    m.VX = -m.VX;
                    m.Angle = Math.PI - m.Angle;
                    x = 0;
                }
                else if (x + MoleculeSize >= BoxWidth)
                {
                    m.VX = -m.VX;
                    m.Angle = Math.PI - m.Angle;
                    x = BoxWidth - MoleculeSize;
                }

                // 상하 벽 충돌 처리
                if (y <= 0)
                {
                    m.VY = -m.VY;
                    m.Angle = -m.Angle;
                    y = 0;
                }
                else if (y + MoleculeSize >= BoxHeight)
                {
                    m.VY = -m.VY;
                    m.Angle = -m.Angle;
                    y = BoxHeight - MoleculeSize;
                }

                AbsoluteLayout.SetLayoutBounds(m.View, new Rect(x, y, MoleculeSize, MoleculeSize));
            }
        }
    }
}
