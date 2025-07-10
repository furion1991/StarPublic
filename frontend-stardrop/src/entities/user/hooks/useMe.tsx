'use client'

import { useQuery } from '@tanstack/react-query'
import { getMe } from '../api/user'
import { InventoryItemStatus } from '../api/user.types'

export const useMe = () => {
  return useQuery({
    queryFn: getMe,
    queryKey: ['me'],
    select: (data) => {
      const { userInventory } = data

      // newest first
      const userInventoryItems = userInventory.itemsUserInventory.reverse()

      const availableInventoryItems = userInventoryItems.filter(
        ({ itemRecordState: itemStatus }) => {
          const forbiddenItemStatusesForActions = [
            InventoryItemStatus.WITHDRAWN,
            InventoryItemStatus.SOLD,
            InventoryItemStatus.USED_ON_UPGRADE,
            InventoryItemStatus.USED_ON_CONTRACT
          ]

          return !forbiddenItemStatusesForActions.includes(itemStatus)
        }
      )

      return {
        ...data,
        userInventory: {
          ...userInventory,
          itemsUserInventory: userInventoryItems,
          availableInventoryItems
        }
      }
    }
  })
}
