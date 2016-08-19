using LandSky.MyEnums;
using System;
using System.Threading;

namespace LandSky.Commands
{
    internal class MoveCommand : BaseCommand
    {
        public MoveDirection Direction;
        public int Steps;
        public static CancellationToken CancleMove;
        public static CancellationTokenSource InvokeCancle;

        public MoveCommand(MoveDirection Direction, int steps)
        {
            this.Direction = Direction;
            this.Steps = steps;
            InitToken();
        }

        public static void InitToken()
        {
            InvokeCancle?.Cancel();
            InvokeCancle = new CancellationTokenSource();
            CancleMove = InvokeCancle.Token;
        }

        public static MoveCommand Phrase(Comands MoveCommand)
        {
            switch (MoveCommand)
            {
                case Comands.Left:
                    return Left(1);

                case Comands.TenStepsLeft:
                    return Left(10);

                case Comands.Right:
                    return Right(1);

                case Comands.TenStepsRight:
                    return Right(10);

                case Comands.Up:
                    return Up(1);

                case Comands.TenStepsUp:
                    return Left(10);

                case Comands.Down:
                    return Down(1);

                case Comands.TenStepsDown:
                    return Down(10);

                default:
                    throw new Exception($"Command {MoveCommand} not valid");
            }
        }

        public static MoveCommand Left(int steps)
        {
            return new MoveCommand(MoveDirection.Left, steps);
        }

        public static MoveCommand UpLeft(int steps)
        {
            return new MoveCommand(MoveDirection.UpLeft, steps);
        }

        public static MoveCommand Up(int steps)
        {
            return new MoveCommand(MoveDirection.Up, steps);
        }

        public static MoveCommand UpRight(int steps)
        {
            return new MoveCommand(MoveDirection.UpRight, steps);
        }

        public static MoveCommand Right(int steps)
        {
            return new MoveCommand(MoveDirection.Right, steps);
        }

        public static MoveCommand DownRight(int steps)
        {
            return new MoveCommand(MoveDirection.DownRight, steps);
        }

        public static MoveCommand Down(int steps)
        {
            return new MoveCommand(MoveDirection.Down, steps);
        }

        public static MoveCommand DownLeft(int steps)
        {
            return new MoveCommand(MoveDirection.DownLeft, steps);
        }
    }
}