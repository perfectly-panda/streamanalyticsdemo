export default class Anomalies {
    timeStamp!: string;
    machineId!: number;
    broken!: boolean;
    failures!: Date;
    capacity!: Date;
    changePointScore!: number;
    isChangePointAnomaly!: number;
    correctDetection!: number;
}