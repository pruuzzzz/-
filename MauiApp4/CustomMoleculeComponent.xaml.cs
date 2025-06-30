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

            // MessagingCenter���� �µ� �� ������ ����
            MessagingCenter.Subscribe<MainPage, int>(this, "UpdateTemperature", (sender, t) =>
            {
                // ���ŵ� �µ� �� ����
                temperature = t;  // �ҹ��� t ���
                UpdateSpeedFromTemperature();  // �µ� ���� ����Ǿ��� ���� �ӵ� ����
            });

            // ���� �ʱ�ȭ�մϴ�
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
                    VX = (rand.NextDouble() - 0.5) * 4, // �ʱ� �ӵ� ����
                    VY = (rand.NextDouble() - 0.5) * 4  // �ʱ� �ӵ� ����
                });
            }

            // Timer ����, 16ms���� UpdateMolecules ȣ��
            timer = new System.Timers.Timer(16) { AutoReset = true };
            timer.Elapsed += (s, e) => Dispatcher.Dispatch(UpdateMolecules);
            timer.Start();
        }

        void UpdateSpeedFromTemperature()
        {
            // �µ� ���� ���� �ǽð����� �ӵ� ����
            foreach (var m in molecules)
            {
                m.VX = temperature / 170.0;  // VX�� t ���� 100���� 1
                m.VY = temperature / 100.0;  // VY�� t ���� 200���� 1
            }
        }

        // �ܺο��� �� �ӵ� ���� �����Ͽ� ���� �ӵ� ����
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
                // ���� ��ġ ������Ʈ
                var b = AbsoluteLayout.GetLayoutBounds(m.View);
                double x = b.X + m.VX;
                double y = b.Y + m.VY;

                // ���� ���� �ε����� VX�� ��ȣ ����
                if (x <= 0) // ���� ���� �ε����� ��
                {
                    m.VX = -m.VX;  // VX�� ��ȣ ����
                    x = 0;  // ��ġ�� ���� �°� ����
                }
                else if (x + MoleculeSize >= BoxWidth) // ������ ���� �ε����� ��
                {
                    m.VX = -m.VX;  // VX�� ��ȣ ����
                    x = BoxWidth - MoleculeSize;  // ��ġ�� ���� �°� ����
                }

                // ���� ���� �ε����� VY�� ��ȣ ����
                if (y <= 0) // ���� ���� �ε����� ��
                {
                    m.VY = -m.VY;  // VY�� ��ȣ ����
                    y = 0;  // ��ġ�� ���� �°� ����
                }
                else if (y + MoleculeSize >= BoxHeight) // �Ʒ��� ���� �ε����� ��
                {
                    m.VY = -m.VY;  // VY�� ��ȣ ����
                    y = BoxHeight - MoleculeSize;  // ��ġ�� ���� �°� ����
                }

                // ��ġ ������Ʈ (��ġ�� �״�� �ΰ�, �ӵ��� ����)
                AbsoluteLayout.SetLayoutBounds(m.View, new Rect(x, y, MoleculeSize, MoleculeSize));
            }
        }
    }
}
