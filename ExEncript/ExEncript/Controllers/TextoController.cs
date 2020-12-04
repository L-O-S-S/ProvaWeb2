using ExEncript.Context;
using ExEncript.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Security.Cryptography;
using System.Text;
using System.Web;
using System.Web.Mvc;

namespace ExEncript.Controllers
{
    public class TextoController : Controller
    {
        // GET: Texto
        private Contexto db = new Contexto();
        private static string AesIV256BD = @"%j?TmFP6$BbMnY$@";
        private static string AesKey256BD = @"rxmBUJy]&,;3jKwDTzf(cui$<nc2EQr)";
        // GET: Mensagem
        public ActionResult Index()
        {
            List<TextoModel> texto = db.Textos.ToList();

            return View(texto.ToList());
        }

        #region Create - GET

        [HttpGet]
        public ActionResult Create(string? msgEncript, string? msgDecript)
        {
            if (msgEncript != null)
            {
                TempData["msgEncript"] = msgEncript;
                TempData["msgDecript"] = msgDecript;
            }

            return View();
        }
        #endregion

        #region Create - POST

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(TextoModel textoModel)
        {
            if (ModelState.IsValid)
            {
                string msgDecript = textoModel.Mensagem;


                //AesCryptoServiceProvider
                AesCryptoServiceProvider aes = new AesCryptoServiceProvider();
                aes.BlockSize = 128;
                aes.KeySize = 256;
                aes.IV = Encoding.UTF8.GetBytes(AesIV256BD);
                aes.Key = Encoding.UTF8.GetBytes(AesKey256BD);
                aes.Mode = CipherMode.CBC;
                aes.Padding = PaddingMode.PKCS7;

                // Convertendo String para byte Arrey
                byte[] src = Encoding.Unicode.GetBytes(textoModel.Mensagem);

                //Encriptação
                using (ICryptoTransform encrypt = aes.CreateEncryptor())
                {
                    byte[] dest = encrypt.TransformFinalBlock(src, 0, src.Length);

                    textoModel.Mensagem = Convert.ToBase64String(dest);

                }
                string msgEncript = textoModel.Mensagem;
                db.Textos.Add(textoModel);
                db.SaveChanges();

                return RedirectToAction(nameof(Create), new { msgEncript = msgEncript, @msgDecript = msgDecript });
            }
            return RedirectToAction(nameof(Create));
        }
        #endregion

        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            TextoModel textoModel = db.Textos.Find(id);
            if (textoModel == null)
            {
                return HttpNotFound();
            }
            string msgEnc = textoModel.Mensagem;
            AesCryptoServiceProvider aes = new AesCryptoServiceProvider();
            aes.BlockSize = 128;
            aes.KeySize = 256;
            aes.IV = Encoding.UTF8.GetBytes(AesIV256BD);
            aes.Key = Encoding.UTF8.GetBytes(AesKey256BD);
            aes.Mode = CipherMode.CBC;
            aes.Padding = PaddingMode.PKCS7;
            
            byte[] src = Convert.FromBase64String(textoModel.Mensagem);
            using (ICryptoTransform decrypt = aes.CreateDecryptor())
            {
                byte[] dest = decrypt.TransformFinalBlock(src, 0, src.Length);
                textoModel.Mensagem = Encoding.Unicode.GetString(dest);
            }
            string msgDec = textoModel.Mensagem;
            TempData["msgEnc"] = msgEnc;
            TempData["msgDec"] = msgDec;
          
            return View();
        }
    }
}
