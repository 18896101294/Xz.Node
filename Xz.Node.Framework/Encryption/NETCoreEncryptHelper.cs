using NETCore.Encrypt;

namespace Xz.Node.Framework.Encryption
{
    /// <summary>
    /// 基于 NETCore.Encrypt实现。目前NETCore.Encrypt只支持.NET Core ,工具包含了AES,DES,RSA加密解密，MD5，SHA*,HMAC*等常用Hash操作。
    /// 目前只是简单用了一下，后续如果需要拓展再参考文档进行使用
    /// Install-Package NETCore.Encrypt -Version 2.0.8
    /// https://github.com/myloveCc/NETCore.Encrypt
    /// </summary>
    public class NETCoreEncryptHelper
    {
        //密钥
        private static string encryptKey = "4h!@w$rng,i#$@x1%)5^3(7*5P31/Ee0";

        //默认密钥向量
        private static byte[] Keys = { 0x41, 0x72, 0x65, 0x79, 0x6F, 0x75, 0x6D, 0x79, 0x53, 0x6E, 0x6F, 0x77, 0x6D, 0x61, 0x6E, 0x3F };

        /// <summary>
        /// AES 加密
        /// </summary>
        /// <param name="decryptString"></param>
        /// <returns></returns>
        public static string AESEncrypt(string decryptString)
        {
            var encrypted = EncryptProvider.AESEncrypt(decryptString, encryptKey); // 不带向量的加密方式，带向量方式参考重载方法
            return encrypted;
        }

        /// <summary>
        /// AES 解密
        /// </summary>
        /// <param name="decryptString"></param>
        /// <returns></returns>
        public static string AESDecrypt(string decryptString)
        {
            var decrypted = EncryptProvider.AESDecrypt(decryptString, encryptKey); // 不带向量的解密方式，带向量方式参考重载方法
            return decrypted;
        }
    }
}
