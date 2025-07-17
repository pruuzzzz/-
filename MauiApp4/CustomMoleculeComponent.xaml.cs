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
            public double Angle { get; set; }  // ���� ����
        }

        readonly List<Molecule> molecules = new();
        readonly System.Timers.Timer timer;
        Random rand = new Random();

        private int temperature = 100; // �⺻ �µ� (ó������ �����̰� �Ϸ��� �ʿ�)

        public CustomMoleculeComponent()
        {
            InitializeComponent();

            // �µ� ���� �޽��� ����
            MessagingCenter.Subscribe<MainPage, int>(this, "UpdateTemperature", (sender, t) =>
            {
                temperature = t;
                UpdateSpeedFromTemperature();
            });

            // �� �ʱ�ȭ
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

            // Ÿ�̸� ����
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

                // �¿� �� �浹 ó��
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

                // ���� �� �浹 ó��
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
