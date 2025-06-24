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
        const double maxX = 250; // �簢�� ũ�� (����)
        const double maxY = 250; // �簢�� ũ�� (����)

        double speedX = 0;  // ���� ������ �ӵ� X ����
        double speedY = 0;  // ���� ������ �ӵ� Y ����

        public CustomMoleculeComponent()
        {
            InitializeComponent();
            CreateMoleculeModels();
        }

        private void CreateMoleculeModels()
        {
            // ���� ���� ���� 20�� ����
            for (int i = 0; i < 20; i++)
            {
                var circle = new BoxView
                {
                    Color = Colors.Blue,
                    WidthRequest = 20,
                    HeightRequest = 20,
                    CornerRadius = 10,
                };

                // ���� ��ġ ���� (�簢�� ���� ��������)
                double randomX = rand.Next(0, (int)(maxX - 20));
                double randomY = rand.Next(0, (int)(maxY - 20));
                AbsoluteLayout.SetLayoutBounds(circle, new Rect(randomX, randomY, 20, 20)); // `Rect` ���

                // �ִϸ��̼� �߰�
                AnimateMolecule(circle);

                AbsoluteLayout.SetLayoutFlags(circle, Microsoft.Maui.Layouts.AbsoluteLayoutFlags.PositionProportional);

                // AbsoluteLayout�� ���� �߰�
                MoleculeLayout.Children.Add(circle);
            }
        }

        private async void AnimateMolecule(BoxView circle)
        {
            speedX = rand.Next(2, 5);  // X�� �ӵ� �ʱ�ȭ
            speedY = rand.Next(2, 5);  // Y�� �ӵ� �ʱ�ȭ

            while (true)
            {
                var currentBounds = AbsoluteLayout.GetLayoutBounds(circle);
                double posX = currentBounds.X;
                double posY = currentBounds.Y;

                // ���� �ε����� ƨ�⵵�� ���� ó��
                if (posX <= 0 || posX >= maxX - 20)
                    speedX *= -1; // X ���� ����

                if (posY <= 0 || posY >= maxY - 20)
                    speedY *= -1; // Y ���� ����

                // ��ġ ������Ʈ (�ӵ��� ����)
                posX += speedX;
                posY += speedY;

                // ���ο� ��ġ�� �̵�
                AbsoluteLayout.SetLayoutBounds(circle, new Rect(posX, posY, 20, 20)); // ��ġ�� `Rect`�� ��Ȯ�� ����

                // �浹 ���� �� ����
                HandleCollisions(circle);

                await Task.Delay(10); // ������ �����̰� �ϱ� ���� ������
            }
        }

        private void HandleCollisions(BoxView circle)
        {
            var circles = MoleculeLayout.Children.OfType<BoxView>().Where(x => x != circle).ToList();

            foreach (var otherCircle in circles)
            {
                var otherBounds = AbsoluteLayout.GetLayoutBounds(otherCircle);
                var currentBounds = AbsoluteLayout.GetLayoutBounds(circle);

                // �� ���� ���ڰ� ������ �� (�浹)
                if (Math.Abs(currentBounds.X - otherBounds.X) < 20 && Math.Abs(currentBounds.Y - otherBounds.Y) < 20)
                {
                    // �浹 ���� - ���� �ݴ� �������� ƨ���
                    speedX *= -1;
                    speedY *= -1;
                }
            }
        }
    }
}
