import { API } from '@/shared/api'

import type {
  UpgradeInfoProps,
  CreateUpgradeProps,
  UpgradeInfo,
  CreateUpgradeResponse
} from './upgrades.types'

export const getUpgradeInfo = async ({ attemptedItemId, userItemId }: UpgradeInfoProps) => {
  const { data } = await API.post<UpgradeInfo>('/items/upgrade/preview', {
    userItemId: userItemId,
    attemptedItemId
  })

  return data
}

export const createUpgrade = async ({
  userId,
  inventoryItemId,
  attemptedItemId
}: CreateUpgradeProps) => {
  const { data } = await API.post<CreateUpgradeResponse>('/items/upgrade', {
    userId,
    userInventoryRecordId: inventoryItemId,
    attemptedItemId
  })

  return data.result
}
