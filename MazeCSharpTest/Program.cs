using System;
using System.Collections.Generic;
using System.Text;
using Random = System.Random;
namespace MazeCSharpTest
{
    public enum WallType
    {
        None = 0,

        Up = 1,
        Down = 2,
        Left = 4,
        Right = 8,

        UpDown = 3,
        LeftRight = 12,

        LeftUp = 5,
        LeftDown = 6,
        RightUp = 9,
        RightDown = 10,

        LeftUpRight = 13,
        UpRightDown = 11,
        RigthDownLeft = 14,
        DownLeftUp = 7,

        LeftUpRightDown = 15
    }
    public class WallInfo
    {
        public int x = -1;
        public int y = -1;
        public bool hasWall = true;
        public WallType type = WallType.None;

        public void Set(int x, int y, bool hasWall = true, WallType type = WallType.None)
        {
            this.x = x;
            this.y = y;
            this.hasWall = hasWall;
            this.type = type;
        }

        public override string ToString()
        {
            return "[WallInfo:x=" + x + " y=" + y + " hasWall=" + hasWall + " type=" + type + "]";
        }
    }

    class Maze
    {
        public int _x { get; private set; }
        public int _y { get; private set; }
        public List<WallInfo> _maze = new List<WallInfo>();

        private Random _rand;
        public int _randSeed = 0;

        public Maze()
        {
        }
        ~Maze()
        {

        }
        public bool CreateWall(int w, int h, int randSeed)
        {
            if (w < 1 || h < 1)
            {
                return false;
            }

            //带墙的格子数
            _x = w * 2 + 1;
            _y = h * 2 + 1;

            //初始化墙列表
            InitWallList();

            //初始化随机数
            if (_randSeed != randSeed || _rand == null)
            {
                _rand = new Random(randSeed);
            }

            //创建迷宫

            //

            //跟新类型
            UpdateType();
            return true;
        }
        public bool CreateWall(int w, int h)
        {
            return CreateWall(w, h, _randSeed);
        }
        private void InitWallList()
        {
            int nLerp = _x * _y - _maze.Count;
            if (nLerp > 0)
            {//添加
                while (nLerp > 0)
                {
                    _maze.Add(new WallInfo());
                    nLerp--;
                }
            }
            else if (nLerp < 0)
            {//移除
                _maze.RemoveRange(0, -nLerp);
            }

            //设置第一个
            _maze[0].Set(0, 0);
            //设置其他
            for (int n = 1, length = _maze.Count; n < length; n++)
            {
                _maze[n].Set(n % _x, n / _x);
            }
        }
        private void RandomPrim()
        {
            //墙极限
            int nWidthLimit = _x - 2;
            int nHeightLimit = _y - 2;

            //起点
            int nTarX = 1;
            int nTarY = 1;

            //标记起点
            GetWallInfo(nTarX, nTarY).hasWall = false;

            List<WallInfo> _neighWallInfo = new List<WallInfo>();

            //记录邻墙
            WallInfo wInfo = null;
            wInfo = GetWallInfo(nTarX, nTarY + 1);//Up
            if (wInfo != null)
            {
                _neighWallInfo.Add(wInfo);
            }
            wInfo = GetWallInfo(nTarX, nTarY - 1);//Down
            if (wInfo != null)
            {
                _neighWallInfo.Add(wInfo);
            }
            wInfo = GetWallInfo(nTarX + 1, nTarY);//Right
            if (wInfo != null)
            {
                _neighWallInfo.Add(wInfo);
            }
            wInfo = GetWallInfo(nTarX - 1, nTarY);//Left
            if (wInfo != null)
            {
                _neighWallInfo.Add(wInfo);
            }

            int nIndex = 0;
            while (_neighWallInfo.Count > 0)
            {
                //随机选择一面墙
                nIndex = _rand.Next(_neighWallInfo.Count);
                
                //找出此墙对面的目标格

            }


        }
        public WallInfo GetWallInfo(int x, int y)
        {
            if (x < 0 || x > _x ||
                y < 0 || y > _y)
            {
                return null;
            }

            int index = x + y * _x;
            if (index >= _maze.Count)
            {
                return null;
            }

            return _maze[index];
        }
        private void UpdateType()
        {
            for (int y = 0; y < _y; y++)
            {
                for (int x = 0; x < _x; x++)
                {
                    if (!GetWallInfo(x, y).hasWall)
                    {
                        GetWallInfo(x, y).type = WallType.None;
                        continue;
                    }

                    WallType type = WallType.None;

                    //Left
                    if (x - 1 >= 0 && GetWallInfo(x - 1, y).hasWall)
                    {
                        type |= WallType.Left;
                    }
                    //Right
                    if (x + 1 < _x && GetWallInfo(x + 1, y).hasWall)
                    {
                        type |= WallType.Right;
                    }
                    //Down
                    if (y - 1 >= 0 && GetWallInfo(x, y - 1).hasWall)
                    {
                        type |= WallType.Down;
                    }
                    //Up
                    if (y + 1 < _y && GetWallInfo(x, y + 1).hasWall)
                    {
                        type |= WallType.Up;
                    }

                    GetWallInfo(x, y).type = type;
                }
            }
        }

        //===================Other Tool Function===============
        public string SelectWall(WallInfo info)
        {
            string ch = string.Empty;
            switch (info.type)
            {
                case WallType.None:
                    ch = "  ";
                    break;
                case WallType.Up:
                    ch = "│";
                    break;
                case WallType.Down:
                    ch = "│";
                    break;
                case WallType.Left:
                    ch = "─";
                    break;
                case WallType.Right:
                    ch = "─";
                    break;
                case WallType.UpDown:
                    ch = "│";
                    break;
                case WallType.LeftRight:
                    ch = "─";
                    break;
                case WallType.LeftUp:
                    ch = "┘";
                    break;
                case WallType.LeftDown:
                    ch = "┐";
                    break;
                case WallType.RightUp:
                    ch = "└";
                    break;
                case WallType.RightDown:
                    ch = "┌";
                    break;
                case WallType.LeftUpRight:
                    ch = "┴";
                    break;
                case WallType.UpRightDown:
                    ch = "├";
                    break;
                case WallType.RigthDownLeft:
                    ch = "┬";
                    break;
                case WallType.DownLeftUp:
                    ch = "┤";
                    break;
                case WallType.LeftUpRightDown:
                    ch = "┼";
                    break;
                default:
                    ch = string.Empty;
                    break;
            }

            return ch;
        }
        public string SelectWall(int x, int y)
        {
            if (x > _x - 1 || x < 0 ||
                y > _y - 1 || y < 0)
            {
                return string.Empty;
            }
            return SelectWall(GetWallInfo(x, y));
        }
    }

    class Program
    {
        static void Main(string[] args)
        {
            Maze maze = new Maze();
            maze.CreateWall(10, 20);

            for (int y = maze._y - 1; y >= 0; y--)
            {
                for (int x = 0; x < maze._x; x++)
                {
                    Console.Write(maze.SelectWall(x, y));
                }
                Console.WriteLine();
            }
        }
    }
}
