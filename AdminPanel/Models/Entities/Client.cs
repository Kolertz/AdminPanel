﻿namespace AdminPanel.Models.Entities;

public class Client
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public decimal Balance { get; set; }
    public List<Tag>? Tags { get; set; }
    public List<Payment>? Payments { get; set; }
}