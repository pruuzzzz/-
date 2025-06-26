using Microsoft.Maui.Controls;
using Microsoft.Maui.Graphics;
using System;
using System.Collections.Generic;
using System.Timers;

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

        public CustomMoleculeComponent()
        {
            InitializeComponent();

            var rand = new Random();
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
                    VX = (rand.NextDouble() - 0.5) * 4,
                    VY = (rand.NextDouble() - 0.5) * 4
                });
            }

            // Timer 생성
            timer = new System.Timers.Timer(16) { AutoReset = true };
            timer.Elapsed += (s, e) => Dispatcher.Dispatch(UpdateMolecules);
            timer.Start();

#if ANDROID
            // Android 전용 코드 (예: Intents 사용)
            LaunchIntent();
#endif
        }

        void UpdateMolecules()
        {
            foreach (var m in molecules)
            {
                var b = AbsoluteLayout.GetLayoutBounds(m.View);
                double x = b.X + m.VX;
                double y = b.Y + m.VY;

                if (x <= 0 || x + MoleculeSize >= BoxWidth) m.VX = -m.VX;
                if (y <= 0 || y + MoleculeSize >= BoxHeight) m.VY = -m.VY;

                AbsoluteLayout.SetLayoutBounds(m.View, new Rect(x, y, MoleculeSize, MoleculeSize));
            }
        }

#if ANDROID
        // Android 전용 코드 예시
        private void LaunchIntent()
        {
            // MainActivity를 호출하는 예시 코드
            var context = Android.App.Application.Context; // ApplicationContext
            var intent = new Android.Content.Intent(context, typeof(MainActivity)); // MainActivity 클래스 사용
            context.StartActivity(intent); // MainActivity 시작
        }
#endif
    }
}
