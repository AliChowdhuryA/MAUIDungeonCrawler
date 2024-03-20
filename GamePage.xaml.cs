using System.ComponentModel;
using System.Timers;

namespace DC3;

public class GameObject : INotifyPropertyChanged
{
    public event PropertyChangedEventHandler PropertyChanged;

    public string? Name { get; set; }
    private int x;
    public int X
    {
        get { return x; }
        set
        {
            if (x != value)
            {
                x = value;
                OnPropertyChanged(nameof(X));
                OnPropertyChanged(nameof(Bounds));

            }
        }
    }

    private int y;
    public int Y
    {
        get { return y; }
        set
        {
            if (y != value)
            {
                y = value;
                OnPropertyChanged(nameof(Y));
                OnPropertyChanged(nameof(Bounds));

            }
        }
    }
    public int Width { get; set; }
    public int Height { get; set; }
    private double health;
    public double Health { 
        get { return health; }
        set
        {
            if (health != value)
            {
                health = value;
                OnPropertyChanged(nameof(Health));
            }
        } 
    }
    public Rect Bounds
    {
        get { return new Rect(X, Y, Width, Height); }
    }

    public string Color { get; set; } = "Red";
    public int Damage { get; set; }
    private int speed = 1;
    public int Speed
    {
        get { return speed; }
        set
        {
            if (speed != value)
            {
                speed = value;
                OnPropertyChanged(nameof(Speed));
            }
        }
    }
    public void MoveUp(int distance)
    {
        Y -= distance;
    }

    public void MoveDown(int distance)
    {
        Y += distance;
    }

    public void MoveLeft(int distance)
    {
        X -= distance;
    }

    public void MoveRight(int distance)
    {
        X += distance;
    }
    protected virtual void OnPropertyChanged(string propertyName)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}

public partial class Player : GameObject
{
    public Attack? CurrentAttack { get; set; }

    internal
     bool Intersects(GameObject other)
    {
        return X < other.X + other.Width &&
               X + Width > other.X &&
               Y < other.Y + other.Height &&
               Y + Height > other.Y
               ; 
    }

    private int moveDirectionX;
    public int MoveDirectionX
    {
        get { return moveDirectionX; }
        set
        {
            if (moveDirectionX != value)
            {
                moveDirectionX = value;
                OnPropertyChanged(nameof(MoveDirectionX));
            }
        }
    }

    private int moveDirectionY;
    public int MoveDirectionY
    {
        get { return moveDirectionY; }
        set
        {
            if (moveDirectionY != value)
            {
                moveDirectionY = value;
                OnPropertyChanged(nameof(MoveDirectionY));
            }
        }
    }
    private int attackDirection;
    public int AttackDirection
    {
        get { return attackDirection; }
        set
        {
            if (attackDirection != value)
            {
                attackDirection = value;
                OnPropertyChanged(nameof(AttackDirection));
            }
        }
    }


}

public partial class Enemy : GameObject
{

    public BoxView BoxView { get; set; }

    public void MoveTowardsPlayer(Player player)
    {
        if (player.X < X)
        {
            MoveLeft(Speed);
        }
        else if (player.X > X)
        {
            MoveRight(Speed);
        }

        if (player.Y < Y)
        {
            MoveUp(Speed);
        }
        else if (player.Y > Y)
        {
            MoveDown(Speed);
        }
    }

    internal bool Intersects(GameObject other)
    {
        return X < other.X + other.Width &&
               X + Width > other.X &&
               Y < other.Y + other.Height &&
               Y + Height > other.Y
               ;
    }
}

public partial class Attack : GameObject
{
    internal bool Intersects(GameObject other)
    {
        return X < other.X + other.Width &&
               X + Width > other.X &&
               Y < other.Y + other.Height &&
               Y + Height > other.Y
               ;
    }
}

public class GameModel : INotifyPropertyChanged
{
    public event PropertyChangedEventHandler PropertyChanged;

    private Player player;
    public Player Player
    {
        get { return player; }
        set
        {
            if (player != value)
            {
                player = value;
                OnPropertyChanged(nameof(Player));
            }
        }
    }

    private Enemy[] enemies;
    public Enemy[] Enemies
    {
        get { return enemies; }
        set
        {
            if (enemies != value)
            {
                enemies = value;
                OnPropertyChanged(nameof(Enemies));
            }
        }
    }
    private int level;
    public int Level
    {
        get
        {
            return level;
        }
        set
        {
            if (level != value)
            {
                level = value;
                OnPropertyChanged(nameof(Level));
            }
        }
    }

    protected virtual void OnPropertyChanged(string propertyName)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}

public partial class GamePage : ContentPage
{
    GameModel gameModel;
    System.Timers.Timer gameLoopTimer;

    public GamePage()
    {
        InitializeComponent();

        gameModel = new GameModel
        {
            Player = new Player
            {
                Name = "Player",
                X = 200,
                Y = 50,
                Width = 100,
                Height = 100,
                Health = 1.0,
                Color = "Blue"
            },
            Level = 1

        };

        CreateEnemiesForLevel(gameModel.Level);

        BindingContext = gameModel;


        gameLoopTimer = new System.Timers.Timer(16);
        gameLoopTimer.Elapsed += OnGameLoop;
        gameLoopTimer.Start();
    }
    private void CreateEnemiesForLevel(int level)
    {
        gameModel.Enemies = new Enemy[level];
        for (int i = 0; i < gameModel.Enemies.Length; i++)
        {
            Random random = new Random();

            double xBounds = DeviceDisplay.Current.MainDisplayInfo.Width;
            double yBounds = DeviceDisplay.Current.MainDisplayInfo.Height;

            int xPosition = 60 * i * random.Next(1, 6) + 10;
            int yPosition = 110 * i * random.Next(1, 6) + 10;

            if (xPosition > (int)xBounds)
            {
                xPosition = random.Next(1,(int)xBounds - 100);
            }
            if (yPosition > (int)yBounds)
            {
                yPosition = random.Next(1, (int)yBounds - 100);
            }

            gameModel.Enemies[i] = new Enemy
            {
                Name = $"Enemy {i + 1}",
                X = xPosition,
                Y = yPosition,
                Width = 100,
                Height = 100,
                Health = 100,
                Color = "Red",
                Speed = (int)Math.Min((random.NextDouble() * level + 2), 5)
                
            };
        }

        // Create a BoxView for each enemy
        foreach (var enemy in gameModel.Enemies)
        {
            var enemyBox = new BoxView
            {
                BindingContext = enemy
            };
            enemyBox.SetBinding(BoxView.ColorProperty, "Color");
            enemyBox.SetBinding(AbsoluteLayout.LayoutBoundsProperty, "Bounds");
            GameLayout.Children.Add(enemyBox);
            enemy.BoxView = enemyBox;
        }
    }

    [Obsolete]
    private void OnGameLoop(object sender, ElapsedEventArgs e)
    {
        // Refresh UI
        Device.BeginInvokeOnMainThread(() =>
        {
            List<Enemy> collidedEnemies = new List<Enemy>();

            movePlayer();

            foreach (var enemy in gameModel.Enemies)
            {
                if (gameModel.Player.Intersects(enemy))
                {
                    enemy.MoveLeft(10);
                    collidedEnemies.Add(enemy);
                    gameModel.Player.Health -= 0.1;
                }
                else
                {
                    if (gameModel.Player.CurrentAttack != null && enemy.Intersects(gameModel.Player.CurrentAttack))
                    {
                        collidedEnemies.Add(enemy);
                    }
                   enemy.MoveTowardsPlayer(gameModel.Player);

                }
            }
            foreach (var enemy in collidedEnemies)
            {
                GameLayout.Children.Remove(enemy.BoxView);
                gameModel.Enemies = gameModel.Enemies.Where(e => e != enemy).ToArray();
            }
            // Check if all enemies are gone
            if (gameModel.Enemies.Length == 0)
            {
                // Increment level and create enemies for the next level
                gameModel.Level++;
                CreateEnemiesForLevel(gameModel.Level);

            }

           if (gameModel.Player.Health <= 0)
            {
                gameLoopTimer.Stop();
                gameLoopTimer.Dispose();
                gameLoopTimer = null;
                Navigation.PopAsync();

            }
        });
    }

    private void movePlayer()
    {
        if (gameModel.Player.MoveDirectionX > 0)
        {
            gameModel.Player.MoveRight(gameModel.Player.MoveDirectionX);
        }
        else if (gameModel.Player.MoveDirectionX < 0)
        {
            gameModel.Player.MoveLeft(-gameModel.Player.MoveDirectionX);
        }

        if (gameModel.Player.MoveDirectionY > 0)
        {
            gameModel.Player.MoveDown(gameModel.Player.MoveDirectionY);
        }
        else if (gameModel.Player.MoveDirectionY < 0)
        {
            gameModel.Player.MoveUp(-gameModel.Player.MoveDirectionY);
        }
    }


    private void OnUpButtonClicked(object sender, EventArgs e)
    {
        gameModel.Player.MoveDirectionY = -5;
        gameModel.Player.MoveDirectionX = 0;


    }
    private void OnDownButtonClicked(object sender, EventArgs e)
    {
        gameModel.Player.MoveDirectionY = 5;
        gameModel.Player.MoveDirectionX = 0;

    }
    private void OnLeftButtonClicked(object sender, EventArgs e)
    {
        gameModel.Player.MoveDirectionX = -5;
        gameModel.Player.MoveDirectionY = 0;



    }
    private void OnRightButtonClicked(object sender, EventArgs e)
    {
        gameModel.Player.MoveDirectionX = 5;
        gameModel.Player.MoveDirectionY = 0;

    }
    private void MoveTimer()
    {
        System.Threading.Timer timer = new System.Threading.Timer((state) =>
        {
            gameModel.Player.MoveDirectionX = 0;
            gameModel.Player.MoveDirectionY = 0;


        }, null, 0, 500);



    }

    [Obsolete]
    private void OnAttackButtonClicked(object sender, EventArgs e)
    {
        gameModel.Player.CurrentAttack = new Attack
        {
            Name = "Attack",
            X = gameModel.Player.X - gameModel.Player.Width / 2,
            Y = gameModel.Player.Y - gameModel.Player.Height / 2,
            Width = gameModel.Player.Width*2,
            Height = gameModel.Player.Height * 2,
            Color = "Green",
        };

        var attackBox = new BoxView
        {
            BindingContext = gameModel.Player.CurrentAttack
        };
        attackBox.SetBinding(BoxView.ColorProperty, "Color");
        attackBox.SetBinding(AbsoluteLayout.LayoutBoundsProperty, "Bounds");
        GameLayout.Children.Add(attackBox);
        Device.StartTimer(TimeSpan.FromSeconds(.5), () =>
        {
            GameLayout.Children.Remove(attackBox);
            gameModel.Player.CurrentAttack = null; // Also clear the CurrentAttack
            return false; // return false to stop the timer
        });
        gameModel.Player.MoveDirectionX = 0;
        gameModel.Player.MoveDirectionY = 0;
    }

}