namespace heat_production_optimization
{
    public interface IDataBaseManager
    {
        public bool LoadDbWithInputData(IFormFile formFile);
        public bool LoadDbWithDanfossData(bool summerPeriod);
        public void ClearDatabase();
    }
}
