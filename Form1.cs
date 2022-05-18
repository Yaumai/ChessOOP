using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ChessOOP
{
    public partial class Form1 : Form
    {
        public Image chessSprites;
        //карта фигур по порядку(в спрайте удобнее вырезать xd)
        public int[,] map = new int[8, 8]
        {
            {15,14,13,12,11,13,14,15 },
            {16,16,16,16,16,16,16,16 },
            {0,0,0,0,0,0,0,0 },
            {0,0,0,0,0,0,0,0 },
            {0,0,0,0,0,0,0,0 },
            {0,0,0,0,0,0,0,0 },
            {26,26,26,26,26,26,26,26 },
            {25,24,23,22,21,23,24,25 },
        };
        //массив кнопок для возможных ходов
        public Button[,] butts = new Button[8, 8];
        //переменная для текущего игрока
        public int currPlayer;
        //переменная жля ъранения предыдущей нажатой кнопки
        public Button prevButton;
        //переменная для движения
        public bool isMoving = false;

        public Form1()
        {
            InitializeComponent();
            //Инициализация спрайтов для шахмат
            chessSprites = new Bitmap("C:\\Users\\Professional\\source\\repos\\ChessOOP\\chess.png");
            
            //button1.BackgroundImage = part;

            Init();
        }
        //инициализация доски
        public void Init()
        {
            map = new int[8, 8]
            {
            {15,14,13,12,11,13,14,15 },
            {16,16,16,16,16,16,16,16 },
            {0,0,0,0,0,0,0,0 },
            {0,0,0,0,0,0,0,0 },
            {0,0,0,0,0,0,0,0 },
            {0,0,0,0,0,0,0,0 },
            {26,26,26,26,26,26,26,26 },
            {25,24,23,22,21,23,24,25 },
            };

            //Кнопка рестарта
            this.ClearButton = new System.Windows.Forms.Button();
            this.SuspendLayout();
            this.ClearButton.Location = new System.Drawing.Point(410, 12);
            this.ClearButton.Name = "ClearButton";
            this.ClearButton.Size = new System.Drawing.Size(51, 23);
            this.ClearButton.TabIndex = 0;
            this.ClearButton.Text = "Restart";
            this.ClearButton.UseVisualStyleBackColor = true;
            this.ClearButton.Click += new System.EventHandler(this.ClearButton_Click);
            this.Controls.Add(this.ClearButton);

            //Текст для отображение кто ходит
            this.label1 = new System.Windows.Forms.Label();
            this.SuspendLayout();
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("Roboto Medium", 14.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.label1.Location = new System.Drawing.Point(495, 98);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(115, 23);
            this.label1.TabIndex = 0;
            this.label1.Text = "Ход белых";
            this.Controls.Add(this.label1);

            currPlayer = 1;
            CreateMap();
        }

        public void CreateMap() //заполнение карты кнопками
        {
            for(int i = 0; i < 8; i++)
            {
                for (int j = 0; j < 8; j++)
                {
                    //создание кнопкит
                    butts[i, j] = new Button();

                    Button butt = new Button();
                    butt.Size = new Size(50, 50);
                    //задание позиций на карте
                    butt.Location = new Point(j*50,i*50);


                    //вырезка фигур и добавление в кнопку (1-белые(вверхняя часть картинки) 2-черные)
                    switch (map[i, j]/10)
                    {
                        case 1:
                            //создаем графику и в неё отрисовываем картинку размером 50 на 50 с соответствующими координатами в наборе спрайтов
                            Image part = new Bitmap(50, 50);
                            Graphics g = Graphics.FromImage(part);
                            g.DrawImage(chessSprites, new Rectangle(0, 0, 50, 50), 0+150*(map[i,j]%10-1), 0, 150, 150, GraphicsUnit.Pixel);
                            //в кнопку засовываем изображение
                            butt.BackgroundImage = part;
                            break;
                        case 2:
                            Image part1 = new Bitmap(50, 50);
                            Graphics g1 = Graphics.FromImage(part1);
                            g1.DrawImage(chessSprites, new Rectangle(0, 0, 50, 50), 0 + 150 * (map[i, j] % 10-1), 150, 150, 150, GraphicsUnit.Pixel);
                            butt.BackgroundImage = part1;
                            break;
                    }
                    butt.BackColor = Color.White;
                    //Обработчик на клик
                    butt.Click += new EventHandler(OnFigurePress);
                    this.Controls.Add(butt);

                    //запись кнопки , которую мы создали
                    butts[i, j] = butt;
                }
            }
        }
        //обработка нажатия на кнопку
        public void OnFigurePress(object sender, EventArgs e)
        {
            //если нажали , то очищаем её цвет
            if (prevButton != null)
                prevButton.BackColor = Color.White;

            //засовываем объект в кнопку
            Button pressedButton = sender as Button;

            //pressedButton.Enabled = false;
            //если по клику на карте есть фигура , то красим в красный + принадлежит ли фигура текущему игроку
            if (map[pressedButton.Location.Y / 50, pressedButton.Location.X / 50] != 0 && map[pressedButton.Location.Y / 50, pressedButton.Location.X / 50]/10 == currPlayer)
            {
                //закрытие всех предыдущих шагов
                CloseSteps();
                //при нажатии на фигуру красный цвет
                pressedButton.BackColor = Color.Red;
                //деактивация всех кнопок
                DeactivateAllButtons();
                //показ текущей кнопки
                pressedButton.Enabled = true;
                //показ кнопок куда можно походить
                ShowSteps(pressedButton.Location.Y / 50, pressedButton.Location.X / 50, map[pressedButton.Location.Y / 50, pressedButton.Location.X / 50]);

                //если фигура в движении(нажатая)
                if (isMoving)
                {
                    CloseSteps();
                    pressedButton.BackColor = Color.White;
                    ActivateAllButtons();
                    //заканчиваем ходить
                    isMoving = false;
                }
                else
                    isMoving = true;
            // если нет , то меняем местами
            }else
            {

                if (isMoving)
                {
                    int temp = map[pressedButton.Location.Y / 50, pressedButton.Location.X / 50];
                    map[pressedButton.Location.Y / 50, pressedButton.Location.X / 50] = map[prevButton.Location.Y / 50, prevButton.Location.X / 50];
                    map[prevButton.Location.Y / 50, prevButton.Location.X / 50] = temp;
                    pressedButton.BackgroundImage = prevButton.BackgroundImage;
                    prevButton.BackgroundImage = null;
                    isMoving = false;
                    CloseSteps();
                    ActivateAllButtons();
                    SwitchPlayer();
                }
            }
           //переприсвоение при нажатии на другую фигуру
            prevButton = pressedButton;
        }

        //показ шагов от выбранной фигуры
        public void ShowSteps(int IcurrFigure, int JcurrFigure, int currFigure)
        {
            //перменная для показа шагов пешки, если 1 - вниз , 2 - вверх
            int dir = currPlayer == 1 ? 1 : -1;
            //проверка какая это фигура
            switch (currFigure%10)
            {
                //пешка
                case 6:
                    //если мы находимся в границах карты
                    if (InsideBorder(IcurrFigure + 1 * dir, JcurrFigure))
                    {
                        //если элемент внизу либо сверху нулевой , то выделяем возможные шаги желтым + включаем кнопку
                        if (map[IcurrFigure + 1 * dir, JcurrFigure] == 0)
                        {
                            butts[IcurrFigure + 1 * dir, JcurrFigure].BackColor = Color.Yellow;
                            butts[IcurrFigure + 1 * dir, JcurrFigure].Enabled = true;
                        }
                    }
                    
                    //проверка на то , есть ли справа либо слева вражеская фигура
                    if (InsideBorder(IcurrFigure + 1 * dir, JcurrFigure+1))
                    {
                        //если в той ячейке что-то находится И это фигура противника
                        if (map[IcurrFigure + 1 * dir, JcurrFigure + 1] != 0 && map[IcurrFigure + 1 * dir, JcurrFigure + 1] / 10 != currPlayer)
                        {
                            butts[IcurrFigure + 1 * dir, JcurrFigure + 1].BackColor = Color.Yellow;
                            butts[IcurrFigure + 1 * dir, JcurrFigure + 1].Enabled = true;
                        }
                    }
                        //то же самое, только слева
                    if (InsideBorder(IcurrFigure + 1 * dir, JcurrFigure - 1))
                    {
                        if (map[IcurrFigure + 1 * dir, JcurrFigure - 1] != 0 && map[IcurrFigure + 1 * dir, JcurrFigure - 1] / 10 != currPlayer)
                        {
                            butts[IcurrFigure + 1 * dir, JcurrFigure - 1].BackColor = Color.Yellow;
                            butts[IcurrFigure + 1 * dir, JcurrFigure - 1].Enabled = true;
                        }
                    }
                    break;
                case 5:
                    // функция для всевозможных путей вверх , вниз , вправо и слево
                    ShowVerticalHorizontal(IcurrFigure, JcurrFigure);
                    break;
                case 3:
                    ShowDiagonal(IcurrFigure, JcurrFigure);
                    break;
                case 2:
                    ShowVerticalHorizontal(IcurrFigure, JcurrFigure);
                    ShowDiagonal(IcurrFigure, JcurrFigure);
                    break;
                case 1:
                    ShowVerticalHorizontal(IcurrFigure, JcurrFigure,true);
                    ShowDiagonal(IcurrFigure, JcurrFigure,true);
                    break;
                case 4:
                    ShowHorseSteps(IcurrFigure, JcurrFigure);
                    break;
            }
        }
        //логика для хождения конем
        public void ShowHorseSteps(int IcurrFigure, int JcurrFigure)
        {
            if (InsideBorder(IcurrFigure - 2, JcurrFigure + 1))
            {
                DeterminePath(IcurrFigure - 2, JcurrFigure + 1);
            }
            if (InsideBorder(IcurrFigure - 2, JcurrFigure - 1))
            {
                DeterminePath(IcurrFigure - 2, JcurrFigure - 1);
            }
            if (InsideBorder(IcurrFigure + 2, JcurrFigure + 1))
            {
                DeterminePath(IcurrFigure + 2, JcurrFigure + 1);
            }
            if (InsideBorder(IcurrFigure + 2, JcurrFigure - 1))
            {
                DeterminePath(IcurrFigure + 2, JcurrFigure - 1);
            }
            if (InsideBorder(IcurrFigure - 1, JcurrFigure + 2))
            {
                DeterminePath(IcurrFigure - 1, JcurrFigure + 2);
            }
            if (InsideBorder(IcurrFigure + 1, JcurrFigure + 2))
            {
                DeterminePath(IcurrFigure +1, JcurrFigure + 2);
            }
            if (InsideBorder(IcurrFigure - 1, JcurrFigure - 2))
            {
                DeterminePath(IcurrFigure - 1, JcurrFigure -2);
            }
            if (InsideBorder(IcurrFigure + 1, JcurrFigure - 2))
            {
                DeterminePath(IcurrFigure +1, JcurrFigure -2);
            }
        }

        //выключение всех кнопок
        public void DeactivateAllButtons()
        {
            for (int i = 0; i < 8; i++)
            {
                for (int j = 0; j < 8; j++)
                {
                    butts[i, j].Enabled = false;
                }
            }
        }
        //включение всех кнопок обратно xd
        public void ActivateAllButtons()
        {
            for (int i = 0; i < 8; i++)
            {
                for (int j = 0; j < 8; j++)
                {
                    butts[i, j].Enabled = true;
                }
            }
        }

        public void ShowDiagonal(int IcurrFigure, int JcurrFigure,bool isOneStep=false)
        {
            //переинициализация переменной для того чтобы двигаться по диагонали
            int j = JcurrFigure + 1;
            for(int i = IcurrFigure-1; i >= 0; i--)
            {
                if (InsideBorder(i, j))
                {
                    if (!DeterminePath(i, j))
                        break;
                }
                //достижение края карты
                if (j < 7)
                    j++;
                else break;

                if (isOneStep)
                    break;
            }

            j = JcurrFigure - 1;
            for (int i = IcurrFigure - 1; i >= 0; i--)
            {
                if (InsideBorder(i, j))
                {
                    if (!DeterminePath(i, j))
                        break;
                }
                if (j > 0)
                    j--;
                else break;

                if (isOneStep)
                    break;
            }

            j = JcurrFigure - 1;
            for (int i = IcurrFigure + 1; i < 8; i++)
            {
                if (InsideBorder(i, j))
                {
                    if (!DeterminePath(i, j))
                        break;
                }
                if (j > 0)
                    j--;
                else break;

                if (isOneStep)
                    break;
            }

            j = JcurrFigure + 1;
            for (int i = IcurrFigure + 1; i < 8; i++)
            {
                if (InsideBorder(i, j))
                {
                    if (!DeterminePath(i, j))
                        break;
                }
                if (j <7)
                    j++;
                else break;

                if (isOneStep)
                    break;
            }
        }
        // функция для всевозможных путей вверх , вниз , вправо и слево
        public void ShowVerticalHorizontal(int IcurrFigure, int JcurrFigure,bool isOneStep=false)
        {
            // 4 цикла для каждой стороны
            //вниз
            for (int i = IcurrFigure + 1; i < 8; i++)
            {
                if (InsideBorder(i, JcurrFigure))
                {
                    if (!DeterminePath(i, JcurrFigure))
                        break;
                }
                //переменная для одного шага, если true, то заканчиваем показ шагов по данному направлению
                if (isOneStep)
                    break;
            }
            //вверх
            for (int i = IcurrFigure - 1; i >= 0; i--)
            {
                if (InsideBorder(i, JcurrFigure))
                {
                    if (!DeterminePath(i, JcurrFigure))
                        break;
                }
                if (isOneStep)
                    break;
            }
            //вправо
            for (int j = JcurrFigure + 1; j < 8; j++)
            {
                //находимся ли мы на карте
                if (InsideBorder(IcurrFigure, j))
                {
                    if (!DeterminePath(IcurrFigure, j))
                        break;
                }
                if (isOneStep)
                    break;
            }
            //влево
            for (int j = JcurrFigure - 1; j >= 0; j--)
            {
                if (InsideBorder(IcurrFigure, j))
                {
                    if (!DeterminePath(IcurrFigure, j))
                        break;
                }
                if (isOneStep)
                    break;
            }
        }

        //функция для возможных ходов
        public bool DeterminePath(int IcurrFigure,int j)
        {
            //проверка на пускую ячейку
            if (map[IcurrFigure, j] == 0)
            {
                butts[IcurrFigure, j].BackColor = Color.Yellow;
                butts[IcurrFigure, j].Enabled = true;
            }
            // если фигура вражеская и мы туда можем сходить , то подсвечиваем
            else
            {
                if (map[IcurrFigure, j] / 10 != currPlayer)
                {
                    butts[IcurrFigure, j].BackColor = Color.Yellow;
                    butts[IcurrFigure, j].Enabled = true;
                }
                return false;
            }
            return true;
        }

        //проверка на значения внутри доски
        public bool InsideBorder(int ti,int tj)
        {
            if (ti >= 8 || tj >= 8 || ti < 0 || tj < 0)
                return false;
            return true;
        }

        //сбрасывает все кнопки на белый
        public void CloseSteps()
        {
            for(int i = 0; i < 8; i++)
            {
                for (int j = 0; j < 8; j++)
                {
                    butts[i, j].BackColor = Color.White;
                }
            }
        }
        //Функция для смены игрока
        public void SwitchPlayer()
        {
            if (currPlayer == 1)
            {
                currPlayer = 2;
                this.label1.Text = "Ход черных";
            }


            else
            {
                currPlayer = 1;
                this.label1.Text = "Ход белых";
            }
        }

        //кнопка для рестарта
        private void ClearButton_Click(object sender, EventArgs e)
        {
            //удаляем все элементы
            this.Controls.Clear();
            Init();
        }

    }
}
