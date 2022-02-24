using Simulador.BL.DTOs;
using System;
using System.Collections.Generic;
using System.Web.Mvc;

namespace Simulador.Web.Controllers
{
    public class SimuladorController : Controller
    {
        private static List<OutputDTO> outputs;

        [HttpGet]
        public ActionResult Index()
        {
            return View(outputs);
        }

        [HttpGet]
        public ActionResult Create()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Create(InputDTO inputDTO)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    #region Inicializo variables
                    outputs = new List<OutputDTO>(); 
                    #endregion

                    #region Variables de entrada
                    var vp = inputDTO.ValorPrestamo;
                    var i = inputDTO.Tasa / 100;
                    var n = inputDTO.Plazo;
                    var vs = inputDTO.ValorSeguro;
                    #endregion

                    #region Logica de negocio
                    var cuota = 0.0;
                    var cuotaSeguro = 0.0;

                    // VP * ((i * ((1+i) ^ n)) / (((1+i) ^ n) - 1)) 
                    cuota = vp * ((i * Math.Pow(1 + i, n)) / (Math.Pow(1 + i, n) - 1));
                    cuotaSeguro = cuota + vs;
                    var saldo = vp;

                    for (int j = 0; j <= n; j++)
                    {
                        if (j == 0)
                        {
                            #region Agrego el objeto cuota a la lista del plan de pago
                            var outputFirst = new OutputDTO
                            {
                                NroCuota = j,
                                AbonoInteres = 0,
                                AbonoCapital = 0,
                                Cuota = cuota,
                                CuotaSeguro = cuotaSeguro,
                                Saldo = saldo
                            };
                            outputs.Add(outputFirst);
                            #endregion

                            continue;
                        }

                        var abonoInteres = saldo * i;
                        var abonoCapital = cuota - abonoInteres;
                        saldo -= abonoCapital;

                        #region Agrego el objeto cuota a la lista del plan de pago
                        var output = new OutputDTO
                        {
                            NroCuota = j,
                            AbonoInteres = abonoInteres,
                            AbonoCapital = abonoCapital,
                            Cuota = cuota,
                            CuotaSeguro = cuotaSeguro,
                            Saldo = saldo
                        };
                        outputs.Add(output);
                        #endregion
                    } 
                    #endregion

                    return RedirectToAction(nameof(Index));
                }
            }
            catch (Exception ex)
            {
                ModelState.AddModelError(string.Empty, ex.Message);
            }

            return View(inputDTO);
        }
    }
}