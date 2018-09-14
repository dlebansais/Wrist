namespace NetTools
{
    public static class SecretUuid
    {
        static SecretUuid()
        {
#error Update GuidBytes declaration with your own guid, then remove this error line.
            GuidBytes = new byte[16] { 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00 };
            string GuidString = HashTools.GetString(GuidBytes);
            GuidString = GuidString.Substring(0, 8) + "-" + GuidString.Substring(8, 4) + "-" + GuidString.Substring(12, 4) + "-" + GuidString.Substring(16, 4) + "-" + GuidString.Substring(20, 12);
            Guid = new System.Guid("{" + GuidString + "}");
        }

        public static byte[] GuidBytes { get; private set; }
        public static System.Guid Guid { get; private set; }
    }
}
