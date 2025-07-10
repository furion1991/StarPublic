import Link from 'next/link'

import { Button, NoDataPanel } from '@/shared/ui'
import { ContractHistoryItem } from '@/entities/contracts'

import type { ContractItem } from '@/entities/user'

import classes from './ContractsHistoryList.module.scss'

type ContractsHistoryListProps = {
  contracts: ContractItem[]
}

export const ContractsHistoryList = ({ contracts }: ContractsHistoryListProps) => {
  if (!contracts.length) {
    return (
      <NoDataPanel
        title='Нет контрактов'
        text='Создайте свой первый контракт'
        action={
          <Link href='/contracts'>
            <Button>Контракты ›</Button>
          </Link>
        }
      />
    )
  }

  return (
    <div className={classes.contractsHistoryList}>
      {contracts
        .sort((a, b) => Number(new Date(b.dateOfContract)) - Number(new Date(a.dateOfContract)))
        .map((contractItemProps) => (
          <ContractHistoryItem key={contractItemProps.dateOfContract} {...contractItemProps} />
        ))}
    </div>
  )
}
