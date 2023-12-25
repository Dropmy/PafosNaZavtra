using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using System;

namespace Test01
{
    public partial class Form1 : Form
    {
        private List<ProductCell> dataSource; // Исходные данные
        private List<ProductCell> filteredData; // Отфильтрованные данные

        public Form1()
        {
            InitializeComponent();


            // Обработчик события изменения значения в ComboBox
            comboBox1.SelectedIndexChanged += ComboBox1_SelectedIndexChanged;

            // Инициализируем форму
            LoadData();
        }

        private void LoadData()
        {
            using (var dbContext = new OlegogEntities())
            {
                if (dbContext.Productes.Any() == false)
                {
                    ErrorMessager.ShowError("Товаров нет на складе");
                    return;
                }

                dataSource = new List<ProductCell>();

                foreach (var product in dbContext.Productes)
                {
                    dataSource.Add(new ProductCell()
                    {
                        Id = product.Id,
                        Name = product.Name,
                        Description = product.Description,
                        Price = product.Price,
                        Manufacturer = dbContext.Manufacturers.FirstOrDefault(m => m.Id == product.ManufacturerId).Name,
                        Image = ImagesController.GetImageByPath(product.ImagePath),
                        Discount = product.Discount,
                    });
                }

                // Исходные данные равны всем товарам
                filteredData = new List<ProductCell>(dataSource);

                dataGridView1.DataSource = filteredData;

                var imageColumn = (DataGridViewImageColumn)dataGridView1.Columns["Image"];
                imageColumn.ImageLayout = DataGridViewImageCellLayout.Zoom;
            }

            // Установим счетчик товаров в заголовок DataGridView
            UpdateProductCountLabel();
        }

        private void ComboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            ApplyDiscountRangeFilter();
        }

        private void ApplyDiscountRangeFilter()
        {
            var selectedRange = comboBox1.SelectedItem?.ToString();

            if (string.IsNullOrEmpty(selectedRange))
            {
                // Если не выбран ни один диапазон, отображаем все товары
                filteredData = new List<ProductCell>(dataSource);
            }
            else
            {
                switch (selectedRange)
                {
                    case "Все диапазоны":
                        filteredData = dataSource.Where(p => IsInRange(p.Discount, 0, 100)).ToList();
                        break;
                    case "0-9.99%":
                        filteredData = dataSource.Where(p => IsInRange(p.Discount, 0, 9.99m)).ToList();
                        break;
                    case "10-14.99%":
                        filteredData = dataSource.Where(p => IsInRange(p.Discount, 10, 14.99m)).ToList();
                        break;
                    case "15% и более":
                        filteredData = dataSource.Where(p => IsGreaterOrEqual(p.Discount, 15)).ToList();
                        break;
                }
            }

            // Обновление источника данных DataGridView
            dataGridView1.DataSource = filteredData;

            // Установим счетчик товаров в заголовок DataGridView
            UpdateProductCountLabel();
        }

        private void UpdateProductCountLabel()
        {
            if (dataGridView1.DataSource is List<ProductCell> productList)
            {
                int productCount = productList.Count;
                labelProductCount.Text = $"Товаров: {productCount}";
            }
        }

        private bool IsInRange(string discount, decimal lowerBound, decimal upperBound)
        {
            if (decimal.TryParse(discount.TrimEnd('%'), out var discountValue))
            {
                return discountValue >= lowerBound && discountValue <= upperBound;
            }

            return false;
        }

        private bool IsGreaterOrEqual(string discount, decimal threshold)
        {
            if (decimal.TryParse(discount.TrimEnd('%'), out var discountValue))
            {
                return discountValue >= threshold;
            }

            return false;
        }
    }
}
