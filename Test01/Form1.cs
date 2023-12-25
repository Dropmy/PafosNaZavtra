using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using System;
using System.Drawing;
using System.ComponentModel;

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

            // Установим счетчик товаров в заголовок DataGridView
            UpdateProductCountLabel();


            dataGridView1.CellFormatting += dataGridView1_CellFormatting;
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
            if (filteredData != null)
            {
                int totalProducts = dataSource.Count;
                int displayedProducts = filteredData.Count;

                labelProductCount.Text = $"Товаров {displayedProducts} из {totalProducts}";
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

        private void dataGridView1_CellFormatting(object sender, DataGridViewCellFormattingEventArgs e)
        {
            if (dataGridView1.Columns[e.ColumnIndex].Name == "Discount")
            {
                // Получаем значение ячейки
                object cellValue = e.Value;

                // Проверяем условие и устанавливаем цвет фона
                if (cellValue != null)
                {
                    decimal value = Convert.ToDecimal(cellValue);

                    if (value > 15) // Замените это условие на нужное вам
                    {
                        e.CellStyle.BackColor = Color.Chartreuse;
                        e.CellStyle.ForeColor = Color.White;
                    }
                    else
                    {
                        // Возвращаем стандартные цвета, если условие не выполняется
                        e.CellStyle.BackColor = dataGridView1.DefaultCellStyle.BackColor;
                        e.CellStyle.ForeColor = dataGridView1.DefaultCellStyle.ForeColor;
                    }
                }
            }
        }



        private void comboBoxSortOrder_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void comboBoxSortOrder_SelectedIndexChanged_1(object sender, EventArgs e)
        {
            SortDataGridViewByPrice(comboBoxSortOrder.SelectedItem.ToString());
        }

        private void SortDataGridViewByPrice(string selectedSortOrder)
        {
            List<ProductCell> sortedList;

            switch (selectedSortOrder)
            {
                case "По возрастанию":
                    sortedList = filteredData.OrderBy(p => p.Price).ToList();
                    break;
                case "По убыванию":
                    sortedList = filteredData.OrderByDescending(p => p.Price).ToList();
                    break;
                default:
                    sortedList = filteredData.ToList();
                    break;
            }

            dataGridView1.DataSource = new BindingList<ProductCell>(sortedList);
        }
    }


}
