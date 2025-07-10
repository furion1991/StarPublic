import type { Loot } from '@/entities/loot'

export type UpgradeInfoProps = {
  userItemId: string
  attemptedItemId: string
}

export type CreateUpgradeProps = {
  userId: string
  inventoryItemId: string
  attemptedItemId: string
}

export type UpgradeInfo = {
  chance: number
  coefficient: number
}

type UpgradeResultItem = {
  inventoryRecordId: string
  isItemActive: boolean
  itemState: number
  item: Loot
}

type UpgradeResult = {
  userId: string
  items: [UpgradeResultItem]
}

export type CreateUpgradeResponse = {
  message: string
  statusCode: number
  result: null | UpgradeResult
}
