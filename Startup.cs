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
// using BenchmarkDotNet.Attributes;
// using BenchmarkDotNet.Running;

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

            // var summary = BenchmarkRunner.Run<SingleVsParallell>();
            // System.Console.WriteLine(summary.Reports);
        }
    
        public async void ElectronBootstrap() {
            
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

    public class SingleVsParallell{

        private Cell[,] grid;
        private List<(int i, int j)> emptyNeighbours;
        private int i = 25;
        private int j = 15;

        public SingleVsParallell(){
            emptyNeighbours = new List<(int i, int j)>();
            InitGrid();
        }

        private async void InitGrid(){
            grid = await new GridService().GetGrid();
        }

        // [Benchmark] // Lägg till benchmarks för AsParallell och för samuels metod
        public void AsSingle()
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
                                AsSingle();
                            }
                        }
                    }
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
