using System;
using System.Collections.Generic;
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

        /// <summary>
        /// 是否有墙
        /// </summary>
        public bool hasWall = true;

        /// <summary>
        /// 墙的类型
        /// </summary>
        public WallType type = WallType.None;

        /// <summary>
        /// 状态
        /// </summary>
        public int flag = 0;

        public void Set(int x, int y, bool hasWall = true, int flag = 0, WallType type = WallType.None)
        {
            this.x = x;
            this.y = y;
            this.hasWall = hasWall;
            this.type = type;
            this.flag = flag;
        }

        public override string ToString()
        {
            return "[WallInfo:x=" + x + " y=" + y + " hasWall=" + hasWall + " type=" + type + " flag=" + flag + "]";
        }
    }

    public class Maze
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
        /// 墙总数
        /// </summary>
        public int Count
        {
            get
            {
                return _maze.Count;
            }
        }

        /// <summary>
        /// 获取墙的序号
        /// </summary>
        /// <param name="info">墙信息</param>
        /// <returns></returns>
        public int GetIndex(WallInfo info)
        {
            return info.x + info.y * _x;
        }

        /// <summary>
        /// 获取指定节点的序号
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <returns></returns>
        public int GetIndex(int x, int y)
        {
            return x + y * _x;
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
        /// <returns>迷宫墙信息，不存在则返回null</returns>
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

        /// <summary>
        /// 获取指定序号墙信息
        /// </summary>
        /// <param name="index">指定编号</param>
        /// <returns>如果不存在则返回null</returns>
        public WallInfo this[int index]
        {
            get
            {
                if (index >= _maze.Count || index < 0)
                {
                    return null;
                }
                return _maze[index];
            }
            private set
            {
                if (index >= _maze.Count || index < 0)
                {
                    throw new Exception("Maze>this[int index]:index is OutOfRange to _maze.");
                }
                _maze[index] = value;
            }
        }

        /// <summary>
        /// 使用A*寻路查找路径
        /// </summary>
        /// <param name="start"></param>
        /// <param name="end"></param>
        /// <returns></returns>
        public List<WallInfo> FindPath(WallInfo start, WallInfo end)
        {
            return _findPath.FindPath(this, start, end);
        }

        #region Other Tool Function

#if !OtherTool

        public string SelectWall(WallInfo info)
        {
            if (info.flag == 1)
            {
                return "●";
            }

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

        #endregion Other Tool Function

        #region Private

        private class BlockWallInfo
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

        private Random _rand;
        private List<WallInfo> _maze = new List<WallInfo>();
        private AStarFindPath _findPath = new AStarFindPath();

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

            //初始化
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

        #endregion Private
    }

    public class AStarFindPath
    {
        public class Node
        {
            public WallInfo wallInfo { get; set; }
            public int valSrc { get; set; }
            public int valDis { get; set; }
            public int valSum { get { return valDis + valSrc; } }
            public Node parent { get; set; }

            public Node(WallInfo info)
            {
                this.wallInfo = info;

                this.valDis = 0;
                this.valSrc = 0;
                this.parent = null;
            }

            public Node() : this(null)
            {
            }

            public override string ToString()
            {
                return "[Node:valSrc=" + valSrc + " valDis" + valDis + " valSum" + valSum + " wallInfo=" + wallInfo + "]";
            }
        }

        /// <summary>
        /// 查找路径
        /// </summary>
        /// <param name="maze"></param>
        /// <param name="wStart"></param>
        /// <param name="wStop"></param>
        /// <param name="bMode"></param>
        /// <returns></returns>
        public List<WallInfo> FindPath(Maze maze, WallInfo wStart, WallInfo wStop, bool bMode = false)
        {
            //初始化寻路节点列表
            InitList(maze);

            //起始节点
            Node startNode = _nodeList[maze.GetIndex(wStart)];
            Node endNode = _nodeList[maze.GetIndex(wStop)];

            //未检查列表
            List<Node> openNode = new List<Node>();
            //已检查列表
            List<Node> closeNode = new List<Node>();

            //初始化检查列表
            openNode.Add(startNode);

            Node currentNode = null;
            Node tempNode = null;
            while (openNode.Count > 0)
            {
                //设置当前结点
                currentNode = openNode[0];

                //取得最优节点
                foreach (var item in openNode)
                {
                    if (item.valSum < currentNode.valSum || //总权值最优
                        item.valSum == currentNode.valSum && item.valDis < currentNode.valDis)//或者相同情况下距目标最哦近
                    {
                        currentNode = item;
                    }
                }

                //将最优节点从未检测列表中删除，并添加到已检测列表中
                openNode.Remove(currentNode);
                closeNode.Add(currentNode);

                //如果当前结点为结束节点
                if (currentNode == endNode)
                {//生成并返回路径
                    return GenericPath(startNode, endNode);
                }

                //访问周边节点
                for (int x = -1, y = -1; x <= 1; x++)
                {
                    for (y = -1; y <= 1; y++)
                    {
                        if (x == 0 && y == 0)
                        {//不访问自身节点
                            continue;
                        }

                        if (!bMode && x == y)
                        {//不访问斜对角节点
                            continue;
                        }

                        //获取周边节点实例
                        tempNode = _nodeList[maze.GetIndex(currentNode.wallInfo.x + x, currentNode.wallInfo.y + y)];

                        if (tempNode.wallInfo.hasWall || closeNode.Contains(tempNode))
                        {//当节点为墙或已在已访问列表中时，跳出当前循环
                            continue;
                        }

                        //计算当前结点到startNode的权值
                        int nValSrc = currentNode.valSrc + GetNodeDistance(currentNode, tempNode);

                        if (!openNode.Contains(tempNode) || nValSrc < tempNode.valSrc)
                        {//如果不在未检测列表中或距离起点更近
                            tempNode.valSrc = nValSrc;//更新权值
                            tempNode.valDis = GetNodeDistance(endNode, tempNode);//更新权值
                            tempNode.parent = currentNode;//设置父节点

                            if (!openNode.Contains(tempNode))
                            {//如果没有在检测列表中
                                openNode.Add(tempNode);
                            }
                        }
                    }
                }

                currentNode = null;
                tempNode = null;
            }

            ClearList();//清理列表
            return null;
        }

        #region Privite

        /// <summary>
        /// 迷宫数据扩展后得到的寻路节点列表
        /// </summary>
        private List<Node> _nodeList = new List<Node>();

        /// <summary>
        /// 初始化寻路节点列表
        /// </summary>
        /// <param name="maze"></param>
        private void InitList(Maze maze)
        {
            int nLerp = maze.Count - _nodeList.Count;
            if (nLerp > 0)
            {
                while (nLerp > 0)
                {
                    _nodeList.Add(new Node());

                    --nLerp;
                }
            }
            else if (nLerp < 0)
            {
                _nodeList.RemoveRange(0, -nLerp);
            }

            for (int index = 0, length = _nodeList.Count; index < length; index++)
            {
                _nodeList[index].wallInfo = maze[index];
            }
        }

        /// <summary>
        /// 清理寻路节点列表
        /// </summary>
        private void ClearList()
        {
            foreach (var item in _nodeList)
            {
                item.parent = null;
                item.wallInfo = null;
                item.valDis = 0;
                item.valSrc = 0;
            }
        }

        private List<WallInfo> GenericPath(Node startNode, Node endNode)
        {
            List<WallInfo> path = new List<WallInfo>();

            Node temp = endNode;
            while (temp != startNode)
            {//由后向前遍历链表
                path.Add(temp.wallInfo);
                temp = temp.parent;
            }

            //反转列表
            path.Reverse();

            ClearList();//清理列表
            return path;
        }

        /// <summary>
        /// 获取两个节点之间的权值
        /// </summary>
        /// <param name="currentNode"></param>
        /// <param name="tempNode"></param>
        /// <returns></returns>
        private int GetNodeDistance(Node currentNode, Node tempNode)
        {
            int cntX = Math.Abs(currentNode.wallInfo.x - tempNode.wallInfo.x);
            int cntY = Math.Abs(currentNode.wallInfo.y - tempNode.wallInfo.y);

            if (cntX > cntY)
            {
                return cntY * 14 + (cntX - cntY) * 10;
            }
            else
            {
                return cntX * 14 + (cntY - cntX) * 10;
            }
        }

        #endregion Privite
    }

    internal class Program
    {
        private static void Main(string[] args)
        {
            System.Diagnostics.Stopwatch stopwatch = new System.Diagnostics.Stopwatch();//监视函数执行时间
            Maze maze = new Maze();
            int x, y;
            do
            {
                Console.Write("请输入迷宫宽和高，可选是否显示结果及随机数种子\n（空格分开，例：3 3 true 1， 即3行3列显示迷宫路径且随机数种子为1）：");
                string[] arg = Console.ReadLine().Split(' ');
                if (arg.Length < 2)
                {
                    Console.WriteLine("Arg error >length != 2");
                    continue;
                }
                if (!int.TryParse(arg[0], out x))
                {
                    Console.WriteLine("x 输入错误！例：2 3");
                    continue;
                }
                if (!int.TryParse(arg[1], out y))
                {
                    Console.WriteLine("y 输入错误！例：2 3");
                    continue;
                }

                int seed = 0;
                bool findPath = false;
                if (arg.Length >= 3)
                {
                    if (!bool.TryParse(arg[2], out findPath))
                    {
                        Console.WriteLine("find 输入错误！例：true/false");
                        continue;
                    }
                }
                if (arg.Length == 4)
                {
                    if (!int.TryParse(arg[3], out seed))
                    {
                        Console.WriteLine("seed 输入错误！例：1");
                        continue;
                    }
                }

                stopwatch.Start();
                maze.Create(x, y, seed);
                stopwatch.Stop();
                double createTime = stopwatch.Elapsed.TotalMilliseconds;
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

                double findTime = 0;
                if (findPath)
                {
                    List<WallInfo> path = null;
                    stopwatch.Start();
                    path = maze.FindPath(maze[1, 1], maze[maze._x - 2, maze._y - 2]);
                    stopwatch.Stop();
                    findTime = stopwatch.Elapsed.TotalMilliseconds;
                    stopwatch.Reset();
                    if (path != null)
                    {
                        foreach (var item in path)
                        {
                            item.flag = 1;
                        }
                    }

                    for (int h = maze._y - 1; h >= 0; h--)
                    {
                        for (int w = 0; w < maze._x; w++)
                        {
                            Console.Write(maze.SelectWall(w, h));
                        }
                        Console.WriteLine();
                    }
                    Console.WriteLine();
                }

                string msg = String.Format("x = {0} y = {1} seed = {2} Create Time(ms) > {3} Find Time(ms) > {4}", x, y, seed, createTime, findTime);
                Console.WriteLine(msg);
            } while (true);
        }
    }
}