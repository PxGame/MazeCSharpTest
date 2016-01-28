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
    public class BlockWallInfo
    {
        public WallInfo info { get; private set; }
        public WallType wallDirect { get; private set; }
        public BlockWallInfo(WallInfo info, WallType wallDirect)
        {
            this.info = info;
            this.wallDirect = wallDirect;
        }

        public override string ToString()
        {
            return "[BlockWallInfo:wallDirect=" + wallDirect + " info=" + info + "]";
        }
    }
    class Maze
    {
        /// <summary>
        /// 迷宫宽
        /// </summary>
        public int _x { get; private set; }
        /// <summary>
        /// 迷宫高
        /// </summary>
        public int _y { get; private set; }
        /// <summary>
        /// 随机数种子
        /// </summary>
        public int _randSeed { get; private set; }

        public Maze()
        {
            _x = 0;
            _y = 0;
            _randSeed = 0;
        }
        ~Maze()
        {

        }

        /// <summary>
        /// 创建迷宫
        /// </summary>
        /// <param name="w">宽（>0）</param>
        /// <param name="h">高（>0）</param>
        /// <param name="randSeed">随机数</param>
        /// <returns>是否创建成功</returns>
        public bool Create(int w, int h, int randSeed)
        {
            if (w < 1 || h < 1)
            {
                return false;
            }

            //带墙的格子数
            _x = w * 2 + 1;
            _y = h * 2 + 1;

            //初始化墙列表
            InitList();

            //初始化随机数
            if (_randSeed != randSeed || _rand == null)
            {
                _rand = new Random(randSeed);
            }

            //生成迷宫
            RandomPrim();
            //

            //更新类型
            UpdateType();
            return true;
        }
        /// <summary>
        /// 创建迷宫
        /// </summary>
        /// <param name="w">宽（>0）</param>
        /// <param name="h">高（>0）</param>
        /// <returns>是否创建成功</returns>
        public bool Create(int w, int h)
        {
            return Create(w, h, _randSeed);
        }
        /// <summary>
        /// 获取指定迷宫墙信息
        /// </summary>
        /// <param name="x">横向坐标</param>
        /// <param name="y">纵向坐标</param>
        /// <returns>迷宫墙信息</returns>
        public WallInfo this[int x, int y]
        {
            get
            {
                return GetWallInfo(x, y);
            }
            private set
            {
                WallInfo wInfo = GetWallInfo(x, y);
                if (wInfo != null)
                {
                    wInfo = value;
                }
                else
                {
                    throw new Exception("Maze>this[int x, int y]:GetWallInfo is null.");
                }
            }
        }

        #region Other Tool Function
#if !OtherTool
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
#endif
        #endregion
        #region Private
        private Random _rand;
        private List<WallInfo> _maze = new List<WallInfo>();
        private void InitList()
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

        /// <summary>
        /// 随机普里姆算法生成迷宫
        /// </summary>
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

            List<BlockWallInfo> neighBlockWallInfo = new List<BlockWallInfo>();
            WallInfo tempWall = null;

            //记录邻墙，初始化需要拆的墙
            if (nTarY < nHeightLimit)
            {
                tempWall = GetWallInfo(nTarX, nTarY + 1);//Up
                if (tempWall != null)
                {
                    neighBlockWallInfo.Add(new BlockWallInfo(tempWall, WallType.Up));//记录邻墙及其所在方向
                }
            }
            if (nTarY > 1)
            {
                tempWall = GetWallInfo(nTarX, nTarY - 1);//Down
                if (tempWall != null)
                {
                    neighBlockWallInfo.Add(new BlockWallInfo(tempWall, WallType.Down));
                }
            }
            if (nTarX < nWidthLimit)
            {
                tempWall = GetWallInfo(nTarX + 1, nTarY);//Right
                if (tempWall != null)
                {
                    neighBlockWallInfo.Add(new BlockWallInfo(tempWall, WallType.Right));
                }
            }
            if (nTarX > 1)
            {
                tempWall = GetWallInfo(nTarX - 1, nTarY);//Left
                if (tempWall != null)
                {
                    neighBlockWallInfo.Add(new BlockWallInfo(tempWall, WallType.Left));
                }
            }

            int nIndex = 0;
            BlockWallInfo tempBlackWall = null;
            WallInfo tempWall2 = null;
            while (neighBlockWallInfo.Count > 0)//是否还有未拆的墙
            {
                //随机选择一面墙
                nIndex = _rand.Next(neighBlockWallInfo.Count);

                //找出此墙对面的目标格
                tempBlackWall = neighBlockWallInfo[nIndex];
                switch (tempBlackWall.wallDirect)
                {
                    case WallType.Up:
                        nTarX = tempBlackWall.info.x;
                        nTarY = tempBlackWall.info.y + 1;
                        break;
                    case WallType.Down:
                        nTarX = tempBlackWall.info.x;
                        nTarY = tempBlackWall.info.y - 1;
                        break;
                    case WallType.Left:
                        nTarX = tempBlackWall.info.x - 1;
                        nTarY = tempBlackWall.info.y;
                        break;
                    case WallType.Right:
                        nTarX = tempBlackWall.info.x + 1;
                        nTarY = tempBlackWall.info.y;
                        break;
                }

                tempWall = GetWallInfo(nTarX, nTarY);//获取目标格
                if (tempWall != null && tempWall.hasWall)
                {
                    //连通目标格
                    tempWall.hasWall = false;
                    tempBlackWall.info.hasWall = false;

                    //添加目标格的邻格
                    if (nTarY > 1)//Down
                    {
                        tempWall = GetWallInfo(nTarX, nTarY - 1);//获取目标格下面的邻墙
                        if (tempWall != null && tempWall.hasWall)
                        {
                            tempWall2 = GetWallInfo(nTarX, nTarY - 2);//获取目标格下面邻墙下面的目标格
                            if (tempWall2 != null && tempWall2.hasWall)
                            {
                                neighBlockWallInfo.Add(new BlockWallInfo(tempWall, WallType.Down));//添加邻墙及其方向
                            }
                        }
                    }

                    if (nTarY < nHeightLimit)//Up
                    {
                        tempWall = GetWallInfo(nTarX, nTarY + 1);
                        if (tempWall != null && tempWall.hasWall)
                        {
                            tempWall2 = GetWallInfo(nTarX, nTarY + 2);
                            if (tempWall2 != null && tempWall.hasWall)
                            {
                                neighBlockWallInfo.Add(new BlockWallInfo(tempWall, WallType.Up));
                            }
                        }
                    }

                    if (nTarX < nWidthLimit)//Right
                    {
                        tempWall = GetWallInfo(nTarX + 1, nTarY);
                        if (tempWall != null && tempWall.hasWall)
                        {
                            tempWall2 = GetWallInfo(nTarX + 2, nTarY);
                            if (tempWall2 != null && tempWall2.hasWall)
                            {
                                neighBlockWallInfo.Add(new BlockWallInfo(tempWall, WallType.Right));
                            }
                        }
                    }

                    if (nTarX > 1)//Left
                    {
                        tempWall = GetWallInfo(nTarX - 1, nTarY);
                        if (tempWall != null && tempWall.hasWall)
                        {
                            tempWall2 = GetWallInfo(nTarX - 2, nTarY);
                            if (tempWall2 != null && tempWall2.hasWall)
                            {
                                neighBlockWallInfo.Add(new BlockWallInfo(tempWall, WallType.Left));
                            }
                        }
                    }
                }
                //移除此墙
                neighBlockWallInfo.RemoveAt(nIndex);

                tempWall = null;
                tempWall2 = null;
                tempBlackWall = null;
            }
        }
        private WallInfo GetWallInfo(int x, int y)
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
        #endregion
    }

    class Program
    {
        static void Main(string[] args)
        {
            System.Diagnostics.Stopwatch stopwatch = new System.Diagnostics.Stopwatch();//监视函数执行时间
            Maze maze = new Maze();
            int x, y;
            do
            {
                string[] arg = Console.ReadLine().Split(' ');
                if (arg.Length != 2)
                {
                    Console.WriteLine("Arg error >length != 2");
                    continue;
                }
                x = int.Parse(arg[0]);
                y = int.Parse(arg[1]);

                stopwatch.Start();
                maze.Create(x, y);
                stopwatch.Stop();

                string msg = String.Format("x = {0} y = {1} Create Time(ms) > {2}",x,y, stopwatch.Elapsed.TotalMilliseconds);
                Console.WriteLine(msg);
                stopwatch.Reset();

                for (int h = maze._y - 1; h >= 0; h--)
                {
                    for (int w = 0; w < maze._x; w++)
                    {
                        Console.Write(maze.SelectWall(w, h));
                    }
                    Console.WriteLine();
                }
                Console.WriteLine();
            } while (true);

        }
    }
}
