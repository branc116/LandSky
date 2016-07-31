using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using MultyNetHack;

namespace MultyNetHack.Commands
{
    class MoveCommand : BaseCommand
    {
        public MoveDirection Direction;
        public int Steps;
        public static CancellationToken CancleMove;
        public static CancellationTokenSource InvokeCancle;


        public MoveCommand(MoveDirection Direction, int steps) {
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
            if (MoveCommand == Comands.Left)
                return Left(1);
            else if (MoveCommand == Comands.TenStepsLeft)
                return Left(10);
            else if (MoveCommand == Comands.Right)
                return Right(1);
            else if (MoveCommand == Comands.TenStepsRight)
                return Right(10);
            if (MoveCommand == Comands.Up)
                return Up(1);
            else if (MoveCommand == Comands.TenStepsUp)
                return Left(10);
            else if (MoveCommand == Comands.Down)
                return Down(1);
            else if (MoveCommand == Comands.TenStepsDown)
                return Down(10);
            else throw new Exception(string.Format("Comand {0} not valid", MoveCommand));
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
