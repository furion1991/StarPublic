import type { Loot } from '@/entities/loot'

export type CreateContractProps = {
  inventoryItemsIds: string[]
  userId: string
}

type ApproximateValues = {
  minValue: number
  maxValue: number
}

export type ContractItemApproximateValueResponse = {
  count: number | null
  message: string
  page: null
  result: ApproximateValues
  statusCode: number
}

type ContractResultItem = {
  inventoryRecordId: string
  isItemActive: boolean
  itemState: number
  item: Loot
}

type CreateContractData = {
  userId: string
  items: [ContractResultItem]
}

export type CreateContractResponse = {
  message: string
  statusCode: number
  result: CreateContractData
}
