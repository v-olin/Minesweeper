using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using ElectronNET.API;
using ElectronNET.API.Entities;
using Minesweeper.Data;
using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Running;

namespace Minesweeper
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        // For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddRazorPages();
            services.AddServerSideBlazor();
            services.AddSingleton<GridService>();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Error");
            }

            app.UseStaticFiles();

            app.UseRouting();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapBlazorHub();
                endpoints.MapFallbackToPage("/_Host");
            });

            if (HybridSupport.IsElectronActive)
            {
                ElectronBootstrap();
            }

            var summary = BenchmarkRunner.Run<SingleVsParallell>();
            System.Console.WriteLine(summary.Reports);
        }

        public async void ElectronBootstrap()
        {

            var browserWindow = await Electron.WindowManager.CreateWindowAsync(new BrowserWindowOptions
            {
                Width = 1350,
                Height = 922 + 20, //content thickness + window bar
                Show = false,
                AutoHideMenuBar = true,
                Resizable = false,
                Maximizable = false,
                Icon = Directory.GetCurrentDirectory() + "\\flag.ico" //at runtime current dir is \obj\Host\bin
            });

            await browserWindow.WebContents.Session.ClearCacheAsync();

            browserWindow.OnReadyToShow += () => browserWindow.Show();
        }
    }

    public class SingleVsParallell
    {

        private Cell[,] grid = new Cell[1000, 500];
        private List<(int i, int j)> emptyNeighbours;
        private int i;
        private int j;
        private Queue<(int i, int j)> toVisit;
        private HashSet<(int i, int j)> visited;
        List<(int i, int j)> neighbours = new();

        public SingleVsParallell()
        {
            emptyNeighbours = new List<(int i, int j)>();
            toVisit = new Queue<(int i, int j)>();
            visited = new HashSet<(int i, int j)>();
            i = (int)(grid.GetLength(0) / 2);
            j = (int)(grid.GetLength(1) / 2);
            for (int a = 0; a < grid.GetLength(0); a++)
            {
                for (int b = 0; b < grid.GetLength(1); b++)
                {
                    grid[a, b] = new Cell() { Type = BoxType.Empty };
                }
            }
        }

        [Benchmark]
        public void AsNestedFor()
        {
            emptyNeighbours.Add((i, j));

            (int b, int f)[] offsets = Perimeter(i, j, grid.GetLength(0), grid.GetLength(1));

            for (int x = offsets[0].b; x <= offsets[0].f; x++)
            {
                for (int y = offsets[1].b; y <= offsets[1].f; y++)
                {
                    if (x + y % 2 != 0 && !(x == 0 && y == 0))
                    {
                        int ip = i + x, jp = j + y;
                        if (grid[ip, jp].Type == BoxType.Empty)
                        {
                            if (!emptyNeighbours.Contains((ip, jp)))
                            {
                                emptyNeighbours.Add((ip, jp));
                                AsNestedFor();
                            }
                        }
                    }
                }
            }
        }

        [Benchmark]
        public void AsQueue()
        {
            toVisit.Enqueue((i, j));
            while (toVisit.Count != 0)
            {
                var temp = toVisit.Dequeue();
                if (!visited.Contains(temp))
                {
                    Neighbours(neighbours, i, j, grid.GetLength(0), grid.GetLength(1));
                    for (int k = 0; k < neighbours.Count; k++)
                    {
                        if (grid[neighbours[k].i, neighbours[k].j].Type == BoxType.Empty &&
                            !(neighbours[k].i == i && neighbours[k].j == j) &&
                            !visited.Contains(neighbours[k]))
                        {
                            toVisit.Enqueue(neighbours[k]);
                        }
                    }
                }
                visited.Add(temp);
            }
        }

        private void Neighbours(List<(int i, int j)> neighbours, int i, int j, int maxh, int maxw)
        {
            int[] iOffsets = new int[] { -1, 0, 1 }, jOffsets = new int[] { -1, 0, 1 };

            if (i == 0) iOffsets = new int[] { 0, 1 }; //Enumerable.Range(0,2).ToList();
            else if (i == maxh - 1) iOffsets = new int[] { -1, 0 };

            if (j == 0) jOffsets = new int[] { 0, 1 };
            else if (j == maxw - 1) jOffsets = new int[] { -1, 0 };

            neighbours.Clear();
            foreach (var ip in iOffsets)
            {
                foreach (var jp in jOffsets)
                {
                    neighbours.Add((i + ip, j + jp));
                }
            }
        }

        private (int b, int f)[] Perimeter(int i, int j, int maxh, int maxw) // i = height offset, j = width offset
        {
            (int h, int w)[] offset = { (0, 0), (0, 0) }; //[0] = height offset, [1] = width offset

            if (i == 0) offset[0] = (0, 1); //0 steps up, 1 step up
            else if (i == maxh - 1) offset[0] = (-1, 0);
            else offset[0] = (-1, 1);

            if (j == 0) offset[1] = (0, 1);
            else if (j == maxw - 1) offset[1] = (-1, 0);
            else offset[1] = (-1, 1);

            return offset;
        }
    }
}
