export default class Order {
    public id!: number;
    public widgetCount!: number;
    public createDate!: Date;
    public pendingCount!: number;
    public smashedCount!: number;
    public slashedCount!: number;
    public completedCount!: number;
    public completed!: boolean;
    public completeDate!: Date;
}