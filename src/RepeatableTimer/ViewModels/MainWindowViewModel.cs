using System;
using System.ComponentModel;
using System.Media;
using System.Windows.Input;
using System.Windows.Threading;

namespace RepeatableTimer.ViewModels
{
    public class MainWindowViewModel : INotifyPropertyChanged
    {
        public MainWindowViewModel()
        {
            Initialize();

            Player = new SoundPlayer(Properties.Resources.notification4);

            Timer = new DispatcherTimer { Interval = TimeSpan.FromMilliseconds(50) };
            Timer.Tick += Timer_Tick;

            StartCommand = new StartCommandImpl(() =>
            {
                if (Status != Status.Pause)
                {
                    int.TryParse(Hour, out var hour);
                    int.TryParse(Minute, out var minute);
                    int.TryParse(Second, out var secound);

                    DesignatedTime = new TimeSpan(hour, minute, secound);
                }

                Hour = "0";
                Minute = "0";
                Second = "0";

                Timer.Start();
                StartTime = DateTime.Now;
                Round = CurrentPeriod.ToString() + "/" + Settings.RepeatTimes.ToString();
                Status = Status.Run;
                IsStartEnabled = false;
                IsPauseEnabled = true;
                IsStopEnabled = true;
            });

            StopCommand = new StopCommandImpl(() =>
            {
                OldTimeSpan = new TimeSpan();
                Timer?.Stop();
                Round = string.Empty;
                Status = Status.Stop;
                Initialize();
                IsStartEnabled = true;
                IsPauseEnabled = false;
                IsStopEnabled = false;
            });

            PauseCommand = new PauseCommandImpl(() =>
            {
                OldTimeSpan = OldTimeSpan + ElapsedTimeSpan;
                Timer?.Stop();
                Status = Status.Pause;
                IsStartEnabled = true;
                IsPauseEnabled = false;
                IsStopEnabled = true;
            });
        }

        public ICommand StartCommand { get; private set; }
        public ICommand StopCommand { get; private set; }
        public ICommand PauseCommand { get; private set; }

        public SoundPlayer Player { get; set; }
        public DispatcherTimer Timer { get; set; }

        private static readonly PropertyChangedEventArgs HourPropertyChangedEventArgs = new PropertyChangedEventArgs(nameof(Hour));
        private string _hour;
        public string Hour
        {
            get => _hour;
            set
            {
                if (_hour != value)
                {
                    _hour = value;
                    PropertyChanged?.Invoke(this, HourPropertyChangedEventArgs);
                }
            }
        }

        private static readonly PropertyChangedEventArgs MinutePropertyChangedEventArgs = new PropertyChangedEventArgs(nameof(Minute));
        private string _minute;
        public string Minute
        {
            get => _minute;
            set
            {
                if (_minute != value)
                {
                    _minute = value;
                    PropertyChanged?.Invoke(this, MinutePropertyChangedEventArgs);
                }
            }
        }

        private static readonly PropertyChangedEventArgs SecondPropertyChangedEventArgs = new PropertyChangedEventArgs(nameof(Second));
        private string _secound;
        public string Second
        {
            get => _secound;
            set
            {
                if (_secound != value)
                {
                    _secound = value;
                    PropertyChanged?.Invoke(this, SecondPropertyChangedEventArgs);
                }
            }
        }

        private static readonly PropertyChangedEventArgs RoundPropertyChangedEventArgs = new PropertyChangedEventArgs(nameof(Round));
        private string _round;
        public string Round
        {
            get => _round;
            set
            {
                if (_round != value)
                {
                    _round = value;
                    PropertyChanged?.Invoke(this, RoundPropertyChangedEventArgs);
                }
            }
        }

        private static readonly PropertyChangedEventArgs IsStartEnabledPropertyChangedEventArgs = new PropertyChangedEventArgs(nameof(IsStartEnabled));
        private bool _isStartEnabled;
        public bool IsStartEnabled
        {
            get => _isStartEnabled;
            set
            {
                if (_isStartEnabled != value)
                {
                    _isStartEnabled = value;
                    PropertyChanged?.Invoke(this, IsStartEnabledPropertyChangedEventArgs);
                }
            }
        }

        private static readonly PropertyChangedEventArgs IsStopEnabledPropertyChangedEventArgs = new PropertyChangedEventArgs(nameof(IsStopEnabled));
        private bool _isStopEnabled;
        public bool IsStopEnabled
        {
            get => _isStopEnabled;
            set
            {
                if (_isStopEnabled != value)
                {
                    _isStopEnabled = value;
                    PropertyChanged?.Invoke(this, IsStopEnabledPropertyChangedEventArgs);
                }
            }
        }

        private static readonly PropertyChangedEventArgs IsPauseEnabledPropertyChangedEventArgs = new PropertyChangedEventArgs(nameof(IsPauseEnabled));
        private bool _isPauseEnabled;
        public bool IsPauseEnabled
        {
            get => _isPauseEnabled;
            set
            {
                if (_isPauseEnabled != value)
                {
                    _isPauseEnabled = value;
                    PropertyChanged?.Invoke(this, IsPauseEnabledPropertyChangedEventArgs);
                }
            }
        }

        public int CurrentPeriod { get; set; } = 1;
        public TimeSpan ElapsedTimeSpan { get; set; }
        public TimeSpan OldTimeSpan { get; set; }
        public TimeSpan DesignatedTime { get; private set; }
        public DateTime StartTime { get; private set; }
        public Status Status { get; set; }
        public Settings Settings { get; set; } = new Settings();
        public event PropertyChangedEventHandler PropertyChanged;

        bool first = true;
        private void Timer_Tick(object sender, EventArgs e)
        {
            switch (Status)
            {
                case Status.Run:
                    ElapsedTimeSpan = DateTime.Now - StartTime + OldTimeSpan;

                    var ss = Math.Floor((DesignatedTime - ElapsedTimeSpan).TotalSeconds);
                    if (ss == 3 && first)
                    {
                        Player.Play();
                        first = false;
                    }

                    if (DesignatedTime <= ElapsedTimeSpan)
                    {
                        Timer.Stop();
                        OldTimeSpan = new TimeSpan();
                        first = true;

                        CurrentPeriod++;
                        if (CurrentPeriod <= Settings.RepeatTimes)
                        {
                            Timer.Start();

                            StartTime = DateTime.Now;
                            Round = CurrentPeriod.ToString() + "/" + Settings.RepeatTimes.ToString();
                            Status = Status.Run;
                            break;
                        }

                        IsStartEnabled = true;
                        IsStopEnabled = false;
                        IsPauseEnabled = false;
                        Status = Status.Stop;
                        CurrentPeriod = 1;
                    }

                    Hour = ElapsedTimeSpan.Hours.ToString();
                    Minute = ElapsedTimeSpan.Minutes.ToString();
                    Second = ElapsedTimeSpan.Seconds.ToString();

                    break;

                case Status.Pause:
                    IsStartEnabled = true;
                    IsStopEnabled = true;
                    IsPauseEnabled = false;
                    break;

                case Status.Stop:
                    Timer.Stop();
                    CurrentPeriod = 1;
                    IsStartEnabled = true;
                    IsStopEnabled = false;
                    IsPauseEnabled = false;
                    break;

                default:
                    break;
            }
        }

        private void Initialize()
        {
            Hour = "0";
            Minute = "0";
            Second = "0";
            Status = Status.Stop;
            IsStartEnabled = true;
            IsStopEnabled = false;
            IsPauseEnabled = false;
        }
    }

    public class StartCommandImpl : ICommand
    {
        private readonly Action _execute;

        public StartCommandImpl(Action startAction) => _execute = startAction;

        public event EventHandler CanExecuteChanged;

        public bool CanExecute(object parameter) => true;

        public void Execute(object parameter) => _execute();
    }

    public class StopCommandImpl : ICommand
    {
        private readonly Action _execute;

        public StopCommandImpl(Action stopAction) => _execute = stopAction;

        public event EventHandler CanExecuteChanged;

        public bool CanExecute(object parameter) => true;

        public void Execute(object parameter) => _execute();
    }

    public class PauseCommandImpl : ICommand
    {
        private readonly Action _execute;

        public PauseCommandImpl(Action pauseAction) => _execute = pauseAction;

        public event EventHandler CanExecuteChanged;

        public bool CanExecute(object parameter) => true;

        public void Execute(object parameter) => _execute();
    }

    public class Settings
    {
        public int RepeatTimes { get; set; } = 1;
        public CountMode CountMode { get; set; } = CountMode.CountUp;
    }

    public enum CountMode
    {
        CountUp,
        CountDown
    }

    public enum Status
    {
        Run,
        Pause,
        Stop
    }
}
