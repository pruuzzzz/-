using System;
using Microsoft.Maui.Controls;
using Microsoft.Maui.Controls.Xaml;
using Microsoft.Maui.Graphics;
using System.Threading.Tasks;
using System.Linq;

namespace MauiApp4
{
    public partial class CustomMoleculeComponent : ContentView
    {
        Random rand = new Random();
        const double maxX = 250; // 사각형 크기 (가로)
        const double maxY = 250; // 사각형 크기 (세로)

        double speedX = 0;  // 전역 변수로 속도 X 정의
        double speedY = 0;  // 전역 변수로 속도 Y 정의

        public CustomMoleculeComponent()
        {
            InitializeComponent();
            CreateMoleculeModels();
        }

        private void CreateMoleculeModels()
        {
            // 원형 분자 모형 20개 생성
            for (int i = 0; i < 20; i++)
            {
                var circle = new BoxView
                {
                    Color = Colors.Blue,
                    WidthRequest = 20,
                    HeightRequest = 20,
                    CornerRadius = 10,
                };

                // 랜덤 위치 설정 (사각형 범위 내에서만)
                double randomX = rand.Next(0, (int)(maxX - 20));
                double randomY = rand.Next(0, (int)(maxY - 20));
                AbsoluteLayout.SetLayoutBounds(circle, new Rect(randomX, randomY, 20, 20)); // `Rect` 사용

                // 애니메이션 추가
                AnimateMolecule(circle);

                AbsoluteLayout.SetLayoutFlags(circle, Microsoft.Maui.Layouts.AbsoluteLayoutFlags.PositionProportional);

                // AbsoluteLayout에 원형 추가
                MoleculeLayout.Children.Add(circle);
            }
        }

        private async void AnimateMolecule(BoxView circle)
        {
            speedX = rand.Next(2, 5);  // X축 속도 초기화
            speedY = rand.Next(2, 5);  // Y축 속도 초기화

            while (true)
            {
                var currentBounds = AbsoluteLayout.GetLayoutBounds(circle);
                double posX = currentBounds.X;
                double posY = currentBounds.Y;

                // 벽에 부딪히면 튕기도록 반전 처리
                if (posX <= 0 || posX >= maxX - 20)
                    speedX *= -1; // X 방향 반전

                if (posY <= 0 || posY >= maxY - 20)
                    speedY *= -1; // Y 방향 반전

                // 위치 업데이트 (속도에 따라)
                posX += speedX;
                posY += speedY;

                // 새로운 위치로 이동
                AbsoluteLayout.SetLayoutBounds(circle, new Rect(posX, posY, 20, 20)); // 위치를 `Rect`로 정확히 설정

                // 충돌 감지 및 반응
                HandleCollisions(circle);

                await Task.Delay(10); // 빠르게 움직이게 하기 위해 딜레이
            }
        }

        private void HandleCollisions(BoxView circle)
        {
            var circles = MoleculeLayout.Children.OfType<BoxView>().Where(x => x != circle).ToList();

            foreach (var otherCircle in circles)
            {
                var otherBounds = AbsoluteLayout.GetLayoutBounds(otherCircle);
                var currentBounds = AbsoluteLayout.GetLayoutBounds(circle);

                // 두 원형 분자가 겹쳤을 때 (충돌)
                if (Math.Abs(currentBounds.X - otherBounds.X) < 20 && Math.Abs(currentBounds.Y - otherBounds.Y) < 20)
                {
                    // 충돌 반응 - 서로 반대 방향으로 튕기기
                    speedX *= -1;
                    speedY *= -1;
                }
            }
        }
    }
}
