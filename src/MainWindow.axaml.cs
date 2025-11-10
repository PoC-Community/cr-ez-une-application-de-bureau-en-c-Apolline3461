using System;
using System.Collections.ObjectModel;
using System.IO;
using System.Text.Json;
using Avalonia.Controls;
using Avalonia.Interactivity;
using MsBox.Avalonia;
using MsBox.Avalonia.Enums;
using TodoListApp.Models;
using System.Collections.Generic;

namespace TodoListApp;

public partial class MainWindow : Window
{
    private ObservableCollection<TaskItem> _tasks = new();

    public MainWindow()
    {
        InitializeComponent();
        TaskList.ItemsSource = _tasks;

        AddButton.Click += OnAddClick;
        DeleteButton.Click += OnDeleteClick;
        SaveButton.Click += OnSaveClick;
        LoadTasksFromJson();
    }

    private void OnAddClick(object? sender, RoutedEventArgs e)
    {
        if (!string.IsNullOrWhiteSpace(TaskInput.Text))
        {
            _tasks.Add(new TaskItem { Title = TaskInput.Text, IsCompleted = false });
            TaskInput.Text = string.Empty;
        }
    }

    private void OnDeleteClick(object? sender, RoutedEventArgs e)
    {
        if (TaskList.SelectedItem is TaskItem selected) {
            _tasks.Remove(selected);
        }
    }

    private async void OnSaveClick(object? sender, RoutedEventArgs e)
    {
        try
        {
            var dataDir = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "data");
            if (!Directory.Exists(dataDir))
                Directory.CreateDirectory(dataDir);

            var jsonStr = JsonSerializer.Serialize(_tasks, new JsonSerializerOptions
            {
                WriteIndented = true
            });
            var filePath = Path.Combine(dataDir, "tasks.json");

            File.WriteAllText(filePath, jsonStr);

            await MessageBoxManager
                .GetMessageBoxStandard("Succès", "Tâches sauvegardées avec succès !", ButtonEnum.Ok)
                .ShowAsync();
        }
        catch (Exception ex)
        {
            await MessageBoxManager
                .GetMessageBoxStandard("Erreur", $"Erreur lors de la sauvegarde : {ex.Message}", ButtonEnum.Ok)
                .ShowAsync();
        }
    }

    private async void LoadTasksFromJson()
    {
        try
        {
            var dataDir = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "data");
            var filePath = Path.Combine(dataDir, "tasks.json");

            if (!File.Exists(filePath))
                return;

            var json = File.ReadAllText(filePath);
            var items = JsonSerializer.Deserialize<List<TaskItem>>(json);

            if (items == null)
                return;

            _tasks.Clear();
            foreach (var item in items)
                _tasks.Add(item);
        }
        catch (Exception ex)
        {
            await MessageBoxManager
                .GetMessageBoxStandard("Erreur", $"Erreur lors du chargement : {ex.Message}", ButtonEnum.Ok)
                .ShowAsync();
        }
    }
}
