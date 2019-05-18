export default class Anomalies {
    timeStamp!: string;
    machineId!: number;
    broken!: boolean;
    failures!: number;
    capacity!: Date;
    changePointScore!: number;
    isChangePointAnomaly!: number;
    correctDetection!: number;
}